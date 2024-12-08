using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StarmyKnife.Core.Contracts.Models;

namespace StarmyKnife.Core.Models
{
    public class JSONPathSearcher : IPathSearcher
    {
        private JToken _jToken;

        public bool TryLoadInput(string json, out string error)
        {
            try
            {
                _jToken = JToken.Parse(json);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<string> FindAllNodes(string jsonPath)
        {
            var results = _jToken.SelectTokens(jsonPath);
            return results.Select(r => r.ToString()).ToList();
        }
    }
}
