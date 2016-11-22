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

        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "ontap.in.ua";
        public ICollection<BeerServedInPubs> Parse(Pub pub, string id, string format)
        {
            var result = new List<BeerServedInPubs>();

            var serviceAccountEmail = "ontap-in-ua@api-project-188344924401.iam.gserviceaccount.com";

            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = typeof(GoogleSheetParser).GetTypeInfo().Assembly.GetManifestResourceStream("ontap.key.p12");

            byte[] pvk = ReadFully(resourceStream);

            var certificate = new X509Certificate2(pvk, "notasecret", X509KeyStorageFlags.Exportable);

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

            var formatParts = format.Split(';');
            var volume = formatParts[1];
            var columns = formatParts[2].Split(',');

            // Define request parameters.
            var range = $"{formatParts[0]}:{'A' + columns.Length}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(id, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                //Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    result.Add(new BeerServedInPubs
                    {
                        ServedIn = pub,
                        //TODO: Add parsing
                    });
                }
            }
            else
            {
                //Console.WriteLine("No data found.");
            }
            //Console.Read();

            return result;            
        }
    }
}
