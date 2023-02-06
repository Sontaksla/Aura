using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Aura.Core
{
    public abstract class ProjectBase
    {
        [JsonProperty]
        protected Dictionary<string, BranchBase> Branches;
        public readonly string FullPath;
        public ProjectBase(string fullPath)
        {
            Branches = new Dictionary<string, BranchBase>();
            FullPath = fullPath;
        }
    }
}
