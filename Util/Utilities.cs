using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NickBuhro.Translit;

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
    }
}
