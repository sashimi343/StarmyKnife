using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace StarmyKnife.Core.Helpers
{
    internal static class HtmlEncodingHelper
    {
        private static readonly Regex RegexNamedEntity = new Regex(@"&(?<Name>[a-z][a-z0-9]+);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexNumericEntity = new Regex(@"&#(?<Num>\d+);", RegexOptions.Compiled);
        private static readonly Regex RegexHexEntity = new Regex(@"&#x(?<Num>[0-9a-f]+);", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string SingleQuoteForNamedOrNumeric = "&#39;";
        private const string SingleQuoteForHex = "&#x27;";

        private static readonly Dictionary<string, int> NamedToNumericMapping = new Dictionary<string, int>
        {
            {"quot", 34},
            {"amp", 38},
            {"apos", 39},
            {"lt", 60},
            {"gt", 62},
            {"nbsp", 160},
            {"iexcl", 161},
            {"cent", 162},
            {"pound", 163},
            {"curren", 164},
            {"yen", 165},
            {"brvbar", 166},
            {"sect", 167},
            {"uml", 168},
            {"copy", 169},
            {"ordf", 170},
            {"laquo", 171},
            {"not", 172},
            {"shy", 173},
            {"reg", 174},
            {"macr", 175},
            {"deg", 176},
            {"plusmn", 177},
            {"sup2", 178},
            {"sup3", 179},
            {"acute", 180},
            {"micro", 181},
            {"para", 182},
            {"middot", 183},
            {"cedil", 184},
            {"sup1", 185},
            {"ordm", 186},
            {"raquo", 187},
            {"frac14", 188},
            {"frac12", 189},
            {"frac34", 190},
            {"iquest", 191},
            {"agrave", 192},
            {"aacute", 193},
            {"acirc", 194},
            {"atilde", 195},
            {"auml", 196},
            {"aring", 197},
            {"aelig", 198},
            {"ccedil", 199},
            {"egrave", 200},
            {"eacute", 201},
            {"ecirc", 202},
            {"euml", 203},
            {"igrave", 204},
            {"iacute", 205},
            {"icirc", 206},
            {"iuml", 207},
            {"eth", 208},
            {"ntilde", 209},
            {"ograve", 210},
            {"oacute", 211},
            {"ocirc", 212},
            {"otilde", 213},
            {"ouml", 214},
            {"times", 215},
            {"oslash", 216},
            {"ugrave", 217},
            {"uacute", 218},
            {"ucirc", 219},
            {"uuml", 220},
            {"yacute", 221},
            {"thorn", 222},
            {"szlig", 223},
            {"divide", 247},
            {"yuml", 255},
            {"oelig", 338},
            {"scaron", 352},
            {"fnof", 402},
            {"circ", 710},
            {"tilde", 732},
            {"alpha", 913},
            {"beta", 914},
            {"gamma", 915},
            {"delta", 916},
            {"epsilon", 917},
            {"zeta", 918},
            {"eta", 919},
            {"theta", 920},
            {"iota", 921},
            {"kappa", 922},
            {"lambda", 923},
            {"mu", 924},
            {"nu", 925},
            {"xi", 926},
            {"omicron", 927},
            {"pi", 928},
            {"rho", 929},
            {"sigma", 931},
            {"tau", 932},
            {"upsilon", 933},
            {"phi", 934},
            {"chi", 935},
            {"psi", 936},
            {"omega", 937},
            {"sigmaf", 962},
            {"thetasym", 977},
            {"upsih", 978},
            {"piv", 982},
            {"ensp", 8194},
            {"emsp", 8195},
            {"thinsp", 8201},
            {"zwnj", 8204},
            {"zwj", 8205},
            {"lrm", 8206},
            {"rlm", 8207},
            {"ndash", 8211},
            {"mdash", 8212},
            {"lsquo", 8216},
            {"rsquo", 8217},
            {"sbquo", 8218},
            {"ldquo", 8220},
            {"rdquo", 8221},
            {"bdquo", 8222},
            {"dagger", 8224},
            {"bull", 8226},
            {"hellip", 8230},
            {"permil", 8240},
            {"prime", 8242},
            {"lsaquo", 8249},
            {"rsaquo", 8250},
            {"oline", 8254},
            {"frasl", 8260},
            {"euro", 8364},
            {"image", 8465},
            {"weierp", 8472},
            {"real", 8476},
            {"trade", 8482},
            {"alefsym", 8501},
            {"larr", 8592},
            {"uarr", 8593},
            {"rarr", 8594},
            {"darr", 8595},
            {"harr", 8596},
            {"crarr", 8629},
            {"forall", 8704},
            {"part", 8706},
            {"exist", 8707},
            {"empty", 8709},
            {"nabla", 8711},
            {"isin", 8712},
            {"notin", 8713},
            {"ni", 8715},
            {"prod", 8719},
            {"sum", 8721},
            {"minus", 8722},
            {"lowast", 8727},
            {"radic", 8730},
            {"prop", 8733},
            {"infin", 8734},
            {"ang", 8736},
            {"and", 8743},
            {"or", 8744},
            {"cap", 8745},
            {"cup", 8746},
            {"int", 8747},
            {"there4", 8756},
            {"sim", 8764},
            {"cong", 8773},
            {"asymp", 8776},
            {"ne", 8800},
            {"equiv", 8801},
            {"le", 8804},
            {"ge", 8805},
            {"sub", 8834},
            {"sup", 8835},
            {"nsub", 8836},
            {"sube", 8838},
            {"supe", 8839},
            {"oplus", 8853},
            {"otimes", 8855},
            {"perp", 8869},
            {"sdot", 8901},
            {"lceil", 8968},
            {"rceil", 8969},
            {"lfloor", 8970},
            {"rfloor", 8971},
            {"lang", 9001},
            {"rang", 9002},
            {"loz", 9674},
            {"spades", 9824},
            {"clubs", 9827},
            {"hearts", 9829},
            {"diams", 9830},
        };

        internal static string ToNamedEntities(string input)
        {
            var namedEntities = HttpUtility.HtmlEncode(input);
            return namedEntities;
        }

        internal static string ToNumericEntities(string input, bool convertAllCharacters = false)
        {
            if (convertAllCharacters)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < input.Length; i++)
                {
                    sb.Append($"&#{(int)input[i]};");
                }

                return sb.ToString();
            }
            else
            {
                var namedEntities = ToNamedEntities(input);
                var numericEntities = ConvertNamedToNumeric(namedEntities);
                return numericEntities;
            }
        }

        internal static string ToHexEntities(string input, bool convertAllCharacters = false)
        {
            if (convertAllCharacters)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < input.Length; i++)
                {
                    sb.Append($"&#x{(int)input[i]:X};");
                }

                return sb.ToString();
            }
            else
            {
                var namedEntities = ToNamedEntities(input);
                var hexEntities = ConvertNamedToHex(namedEntities);
                return hexEntities;
            }
        }

        internal static string FromNamedEntities(string input)
        {
            var output = HttpUtility.HtmlDecode(input);
            return output;
        }

        internal static string FromNumericEntities(string input)
        {
            // System.Net.HttpUtility.HtmlDecode also supports numeric entities
            var output = HttpUtility.HtmlDecode(input);
            return output;
        }

        internal static string FromHexEntities(string input)
        {
            var numericEntities = ConvertHexToNumeric(input);
            var output = FromNumericEntities(numericEntities);
            return output;
        }

        private static string ConvertNamedToNumeric(string input)
        {
            var output = RegexNamedEntity.Replace(input, match =>
            {
                if (NamedToNumericMapping.TryGetValue(match.Groups["Name"].Value.ToLower(), out var code))
                {
                    return $"&#{code};";
                }
                else
                {
                    throw new ArgumentException($"Unknown named entity: {match.Value}");
                }
            });

            return output;
        }

        private static string ConvertNamedToHex(string input)
        {
            var convertSingleQuote = input.Replace(SingleQuoteForNamedOrNumeric, SingleQuoteForHex);
            var output = RegexNamedEntity.Replace(convertSingleQuote, match =>
            {
                if (NamedToNumericMapping.TryGetValue(match.Groups["Name"].Value.ToLower(), out var code))
                {
                    return $"&#x{code:X};";
                }
                else
                {
                    throw new ArgumentException($"Unknown named entity: {match.Value}");
                }
            });

            return output;
        }

        private static string ConvertHexToNumeric(string input)
        {
            var output = RegexHexEntity.Replace(input, match =>
            {
                var code = int.Parse(match.Groups["Num"].Value);
                var entity = $"&#{code:X};";
                return entity;
            });

            return output;
        }
    }
}
