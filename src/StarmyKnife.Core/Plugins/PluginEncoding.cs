using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public sealed class PluginEncoding
    {
        public static readonly PluginEncoding AsIs = new("As Is", Encoding.Default);
        public static readonly PluginEncoding Default = new("Default", Encoding.Default);
        public static readonly PluginEncoding ASCII = new("ASCII", Encoding.ASCII);
        public static readonly PluginEncoding UTF8 = new("UTF-8", Encoding.UTF8);
        public static readonly PluginEncoding UTF8N = new("UTF-8N", new UTF8Encoding(true));
        public static readonly PluginEncoding UTF16BE = new("UTF-16 BE", Encoding.BigEndianUnicode);
        public static readonly PluginEncoding UTF16LE = new("UTF-16 LE", Encoding.Unicode);
        public static readonly PluginEncoding UTF32 = new("UTF-32", Encoding.UTF32);
        public static readonly PluginEncoding ShiftJIS = new("Shift-JIS", Encoding.GetEncoding(932));
        public static readonly PluginEncoding EUCJP = new("EUC-JP", Encoding.GetEncoding(51932));
        public static readonly PluginEncoding ISO2022JP = new("ISO-2022-JP", Encoding.GetEncoding(50220));

        public string Name { get; }
        public Encoding Encoding { get; }

        private PluginEncoding(string name, Encoding encoding)
        {
            Name = name;
            Encoding = encoding;
        }

        public static IEnumerable<PluginEncoding> GetAvailableEncodings()
        {
            yield return AsIs;
            yield return Default;
            yield return ASCII;
            yield return UTF8;
            yield return UTF8N;
            yield return UTF16BE;
            yield return UTF16LE;
            yield return UTF32;
            yield return ShiftJIS;
            yield return EUCJP;
            yield return ISO2022JP;
        }

        public static IEnumerable<ListItem> GetListItems()
        {
            return GetListItems(GetAvailableEncodings());
        }

        public static IEnumerable<ListItem> GetListItems(IEnumerable<PluginEncoding> encodings)
        {
            return encodings.Select(e => new ListItem(e.Name, e.Encoding));
        }

        public static IEnumerable<ListItem> GetListItems(params PluginEncoding[] encodings)
        {
            return GetListItems((IEnumerable<PluginEncoding>)encodings);
        }
    }
}
