using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.Contracts.Services;

namespace StarmyKnife.Tests.xUnit.Helpers
{
    public class MockAppPropertiesWrapper : IAppPropertiesWrapper
    {
        private readonly Dictionary<string, object> _properties;

        public MockAppPropertiesWrapper()
        {
            _properties = new Dictionary<string, object>();
        }

        public object this[string key] { get => _properties.GetValueOrDefault(key); set => _properties[key] = value; }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }
    }
}
