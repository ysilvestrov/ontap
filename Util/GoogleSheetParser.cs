using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NuGet.Packaging;
using ontap.Migrations;
using Ontap.Models;

namespace Ontap.Util
{
    public class GoogleSheetParser
    {
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        static string[] Scopes = {SheetsService.Scope.SpreadsheetsReadonly};
        static string ApplicationName = "ontap.in.ua";

        public ICollection<BeerServedInPubs> Parse(Pub pub, 
            IEnumerable<Beer> enumBeers, IEnumerable<Brewery> enumBreweries, 
            Dictionary<string, object> options, Country country)
        {
            var result = new List<BeerServedInPubs>();
            var beers = new List<Beer>(enumBeers);
            var breweries = new List<Brewery>(enumBreweries);

            var serviceAccountEmail = "ontap-in-ua@api-project-188344924401.iam.gserviceaccount.com";

            var certificate =
                new X509Certificate2(
                    ReadFully(typeof(GoogleSheetParser).GetTypeInfo()
                        .Assembly.GetManifestResourceStream("ontap.key.p12")), "notasecret",
                    X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = Scopes
                }.FromCertificate(certificate));

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            decimal volume, targetVolume, priceMultiplicator;
            options["volume"].TryParse(out volume, 1);
            options["targetVolume"].TryParse(out targetVolume, 0.5M);
            options["priceMultiplicator"].TryParse(out priceMultiplicator, 1);
                
            var columns =
                options["columns"].ToString().ToLowerInvariant()
                    .Split(',')
                    .Select((value, index) => new {index, value})
                    .ToDictionary(c => c.value, c => c.index);

            // Define request parameters.
            var range = $"{options["firstCell"]}:{'A' + columns.Count}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(options["sheetId"].ToString(), range);

            IList<IList<object>> values = request.Execute().Values;
            if (values == null || values.Count <= 0) return result;

            //Console.WriteLine("Name, Major");
            foreach (var row in values)
            {
                var serve = new BeerServedInPubs
                {
                    ServedIn = pub,
                    Volume = targetVolume,
                    Updated = DateTime.Now,
                };
                if (columns.Keys.Contains("tap") && columns["tap"] < row.Count)
                {
                    int tap;
                    if (row[columns["tap"]].TryParse(out tap))
                        serve.Tap = tap;
                }
                if (columns.Keys.Contains("price") && columns["price"] < row.Count)
                {
                    decimal price;
                    if (row[columns["price"]].TryParse(out price))
                    {
                        serve.Price = price / priceMultiplicator * (targetVolume / volume);
                    }                        
                }
                if (columns.Keys.Contains("name") && columns.Keys.Contains("brewery") && columns["name"] < row.Count && columns["brewery"] < row.Count)
                {
                    var beerName = row[columns["name"]].ToString();
                    var breweryName = row[columns["brewery"]].ToString();
                    if (!string.Equals(beerName, string.Empty, StringComparison.Ordinal)
                        && !string.Equals(breweryName, string.Empty, StringComparison.Ordinal))
                    {
                        var soundexBeerName = beerName.MakeSoundexKey();
                        var soundexBreweryName = breweryName.MakeSoundexKey();
                        var soundexBeerAndBreweryName = $"{beerName} {breweryName}".MakeSoundexKey();
                        var beer = beers.FirstOrDefault(b => b.Name == beerName && b.Brewery?.Name == breweryName) ??
                                   beers.FirstOrDefault(
                                       b =>
                                           (b.Name.MakeSoundexKey() == soundexBeerName ||
                                            b.Name.MakeSoundexKey() == soundexBeerAndBreweryName)
                                           &&
                                           b.Brewery?.Name.MakeSoundexKey() == soundexBreweryName);
                        if (beer == null)
                        {
                            var brewery = breweries.FirstOrDefault(b => b.Name == breweryName) ??
                                          breweries.FirstOrDefault(b => b.Name.MakeSoundexKey() == soundexBreweryName);
                            if (brewery == null)
                            {
                                brewery = new Brewery
                                {
                                    Name = breweryName,
                                    Country = country,
                                    Id = Utilities.CreateId(breweryName, i => breweries.Any(c => c.Id == i))
                                };
                                breweries.AddRange(new[] {brewery});
                            }

                            beer = new Beer
                            {
                                Name = beerName,
                                Brewery = brewery,
                                Id = Utilities.CreateId(beerName, i => beers.Any(c => c.Id == i))
                            };

                            if (columns.Keys.Contains("alcohol") && columns["alcohol"] < row.Count)
                            {
                                decimal alcohol;
                                if (row[columns["alcohol"]].TryParse(out alcohol))
                                    beer.Alcohol = alcohol;
                            }
                            if (columns.Keys.Contains("ibu") && columns["ibu"] < row.Count)
                            {
                                decimal ibu;
                                if (row[columns["ibu"]].TryParse(out ibu))
                                    beer.Ibu = ibu;
                            }
                            if (columns.Keys.Contains("gravity") && columns["gravity"] < row.Count)
                            {
                                decimal gravity;
                                if (row[columns["gravity"]].TryParse(out gravity))
                                    beer.Gravity = gravity;
                            }
                            if (columns.ContainsKey("description") && columns["description"] < row.Count)
                            {
                                beer.Description = row[columns["description"]].ToString();
                            }
                            beers.AddRange(new[] {beer});
                        }
                        serve.Served = beer;
                    }
                }
                result.Add(serve);
            }


            return result;
        }
    }
}
