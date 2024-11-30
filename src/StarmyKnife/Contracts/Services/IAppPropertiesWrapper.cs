using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Contracts.Services
{
    public interface IAppPropertiesWrapper
    {
        object this[string key] { get; set; }
        bool ContainsKey(string key);
    }
}
