using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Contracts.Models
{
    public interface IPathSearcher
    {
        public bool TryLoadInput(string input, out string error);
        public List<string> FindAllNodes(string path);
    }
}
