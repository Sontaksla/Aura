using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Core
{
    public abstract class BranchBase
    {
        [JsonProperty]
        protected List<CommitBase> Commits;
        public readonly string Name;
        public BranchBase(string name)
        {
            Commits = new List<CommitBase>();
            Name = name;
        }
    }
}
