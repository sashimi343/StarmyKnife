using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class StarmyKnifePluginAttribute : ExportAttribute, IPluginMetadata
    {
        public StarmyKnifePluginAttribute(string name) : base(typeof(PluginBase))
        {
            Name = name;
        }

        public string Name { get; private set; }
        public string Category { get; set; }
    }
}
