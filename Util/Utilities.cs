using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NickBuhro.Translit;
using Phonix;

namespace Ontap.Util
{
    public static class Utilities
    {
        public static string MakeId(this string name)
        {
            return string.Concat(
                Transliteration.CyrillicToLatin(name)
                    .Replace(' ', '_')
                    .Where(ch => char.IsLetterOrDigit(ch) || ch=='_'));
        }

        public static string MakeSoundexKey(this string name)
        {
            return new DoubleMetaphone().BuildKey(Transliteration.CyrillicToLatin(name));
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
    }
}
