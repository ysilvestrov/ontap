using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NickBuhro.Translit;
using Phonix;

namespace Ontap.Util
{
    public static class Utilities
    {
        private static readonly Regex CTester = new Regex("(?:[^A-Za-z0-9,.-])[CcIiPp]+(?:[^A-Za-z0-9,.-])");

        public static string MakeId(this string name)
        {
            return string.Concat(
                Transliteration.CyrillicToLatin(name)
                    .Replace(' ', '_')
                    .Where(ch => char.IsLetterOrDigit(ch) || ch=='_'));
        }

        public static string MakeSoundexKey(this string name)
        {
            name = CTester.Replace(name, match => match.Value.ToLowerInvariant().Replace('c','с').Replace('i', 'і').Replace('p', 'р'));
            var words = new Regex(@"\W+").Split(Transliteration.CyrillicToLatin(name));
            var builder = new DoubleMetaphone(6);
            var result = string.Join("_", words.Select(w => builder.BuildKey(w)).ToArray());
            return result;
        }

        public static bool TryParse(this object source, out decimal value, decimal defaultValue = 0)
        {
            if (decimal.TryParse(
                string.Concat(source.ToString().Where(c => char.IsDigit(c) || c == '.' || c == '-')), 
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value))
                return true;
            value = defaultValue;
            return false;
        }
        public static bool TryParse(this object source, out int value, int defaultValue = 0)
        {
            if (int.TryParse(
                string.Concat(source.ToString().Where(c => char.IsDigit(c) || c == '-')), 
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value))
                return true;
            value = defaultValue;
            return false;
        }

        public static string CreateId(string name, Func<string, bool> test)
        {
            var orgiginalId = name.MakeId();
            var id = orgiginalId;

            for (var i = 0; test(id); id = $"{orgiginalId}_{i}", i++) ;

            return id;
        }

        public static bool TryParseDatabaseUrl(string databaseUrl, out string connectionString)
        {
            if (Uri.TryCreate(databaseUrl, UriKind.Absolute, out var url))
            {
                //"Server=localhost;database=ontap;uid=ontap;pwd=ontap;"
                connectionString = $"Server={url.Host};database={url.LocalPath.Substring(1)};uid={url.UserInfo.Split(':')[0]};pwd={url.UserInfo.Split(':')[1]};";
                return true;
            }
            connectionString = null;
            return false;
        }
    }
}
