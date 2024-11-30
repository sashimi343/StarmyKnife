using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using StarmyKnife.Core.Helpers;
using StarmyKnife.Core.Plugins.Internal;

namespace StarmyKnife.Core.Plugins
{
    public class PluginParametersConfiguration
    {
        private List<IPluginParameter> _parameters;

        internal PluginParametersConfiguration()
        {
            _parameters = new List<IPluginParameter>();
        }

        internal PluginParameterCollection GetParametersSchema()
        {
            var collection = new PluginParameterCollection();

            foreach (var parameter in _parameters)
            {
                collection.Add(parameter);
            }

            _parameters.Clear();

            return collection;
        }

        #region Flag parameter
        public PluginParametersConfiguration AddFlagParameter(string key)
        {
            return AddFlagParameter(key, key, false);
        }

        public PluginParametersConfiguration AddFlagParameter(string key, string name)
        {
            return AddFlagParameter(key, name, false);
        }

        public PluginParametersConfiguration AddFlagParameter(string key, bool defaultValue)
        {
            _parameters.Add(new FlagPluginParameter(key, key, defaultValue));
            return this;
        }

        public PluginParametersConfiguration AddFlagParameter(string key, string name, bool defaultValue)
        {
            _parameters.Add(new FlagPluginParameter(key, name, defaultValue));
            return this;
        }
        #endregion

        #region Text parameter
        public PluginParametersConfiguration AddTextParameter(string key)
        {
            return AddTextParameter(key, key, "");
        }

        public PluginParametersConfiguration AddTextParameter(string key, string name)
        {
            return AddTextParameter(key, name, "");
        }

        public PluginParametersConfiguration AddTextParameter(string key, string name, string defaultValue)
        {
            _parameters.Add(new TextPluginParameter(key, name, defaultValue));
            return this;
        }
        #endregion

        #region List parameter
        public PluginParametersConfiguration AddListParameter<T>(string key) where T : Enum
        {
            return AddListParameter<T>(key, key, default);
        }

        public PluginParametersConfiguration AddListParameter<T>(string key, string name) where T : Enum
        {
            return AddListParameter<T>(key, name, default);
        }

        public PluginParametersConfiguration AddListParameter<T>(string key, T defaultValue) where T : Enum
        {
            var items = Enum.GetValues(typeof(T)).Cast<T>().Select(e => new ListItem(e.GetDisplayName(), e)).ToList();
            var defaultIndex = items.FindIndex(i => i.Value.Equals(defaultValue));
            _parameters.Add(new ListPluginParameter(key, key, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter<T>(string key, string name, T defaultValue) where T : Enum
        {
            var items = Enum.GetValues(typeof(T)).Cast<T>().Select(e => new ListItem(e.GetDisplayName(), e)).ToList();
            var defaultIndex = items.FindIndex(i => i.Value.Equals(defaultValue));
            _parameters.Add(new ListPluginParameter(key, name, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter(string key, IEnumerable<string> labels)
        {
            return AddListParameter(key, key, labels, 0);
        }

        public PluginParametersConfiguration AddListParameter(string key, string name, IEnumerable<string> labels)
        {
            return AddListParameter(key, name, labels, 0);
        }

        public PluginParametersConfiguration AddListParameter(string key, string name, IEnumerable<string> labels, int defaultIndex)
        {
            var items = labels.Select(l => new ListItem(l, l)).ToList();
            _parameters.Add(new ListPluginParameter(key, name, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter(string key, IEnumerable<string> labels, int defaultIndex)
        {
            var items = labels.Select(l => new ListItem(l, l)).ToList();
            _parameters.Add(new ListPluginParameter(key, key, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter(string key, string name, IEnumerable<string> labels, string defaultValue)
        {
            var items = labels.Select(l => new ListItem(l, l)).ToList();
            var defaultIndex = items.FindIndex(i => i.Value.Equals(defaultValue));
            _parameters.Add(new ListPluginParameter(key, name, items, defaultIndex));
            return this;
        }


        public PluginParametersConfiguration AddListParameter(string key, IEnumerable<string> labels, string defaultValue)
        {
            var items = labels.Select(l => new ListItem(l, l)).ToList();
            var defaultIndex = items.FindIndex(i => i.Value.Equals(defaultValue));
            _parameters.Add(new ListPluginParameter(key, key, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter(string key, IEnumerable<ListItem> items)
        {
            return AddListParameter(key, key, items, 0);
        }

        public PluginParametersConfiguration AddListParameter(string key, string name, IEnumerable<ListItem> items)
        {
            return AddListParameter(key, name, items, 0);
        }

        public PluginParametersConfiguration AddListParameter(string key, IEnumerable<ListItem> items, int defaultIndex)
        {
            _parameters.Add(new ListPluginParameter(key, key, items, defaultIndex));
            return this;
        }

        public PluginParametersConfiguration AddListParameter(string key, string name, IEnumerable<ListItem> items, int defaultIndex)
        {
            _parameters.Add(new ListPluginParameter(key, name, items, defaultIndex));
            return this;
        }
        #endregion

        #region Numeric parameter
        public PluginParametersConfiguration AddIntegerParameter(string key)
        {
            return AddIntegerParameter(key, key, 0);
        }

        public PluginParametersConfiguration AddIntegerParameter(string key, string name)
        {
            return AddIntegerParameter(key, name, 0);
        }

        public PluginParametersConfiguration AddIntegerParameter(string key, int defaultValue)
        {
            return AddIntegerParameter(key, key, defaultValue);
        }

        public PluginParametersConfiguration AddIntegerParameter(string key, string name, int defaultValue)
        {
            _parameters.Add(new NumericPluginParameter(key, name, true, defaultValue));
            return this;
        }

        public PluginParametersConfiguration AddDecimalParameter(string key)
        {
            return AddDecimalParameter(key, key, 0);
        }

        public PluginParametersConfiguration AddDecimalParameter(string key, string name)
        {
            return AddDecimalParameter(key, name, 0);
        }

        public PluginParametersConfiguration AddDecimalParameter(string key, decimal defaultValue)
        {
            return AddDecimalParameter(key, key, defaultValue);
        }

        public PluginParametersConfiguration AddDecimalParameter(string key, string name, decimal defaultValue)
        {
            _parameters.Add(new NumericPluginParameter(key, name, false, defaultValue));
            return this;
        }
        #endregion
    }
}
