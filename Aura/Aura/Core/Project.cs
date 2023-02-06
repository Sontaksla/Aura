using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aura.Core
{
    public sealed class Project : ProjectBase
    {
        public new Dictionary<string, Branch> Branches { get; init; }
        [JsonConstructor]
        public Project(string fullPath) : base(fullPath)
        {
            Branches = new Dictionary<string, Branch>();
        }
        public void AddBranch(Branch branch)
        {
            Branches[branch.Name] = branch;
        }
        public Branch? GetBranch(string name)
        {
            if (Branches.ContainsKey(name) == false) return null;

            return Branches[name];
        }
        public IEnumerable<Branch> GetBranches() 
        {
            foreach (var branch in Branches)
            {
                yield return branch.Value;
            }
        }
        public void DeleteBranch(string name)
        {
            if (Branches.ContainsKey(name))
            {
                Branches.Remove(name);
            }
        }
    }
}
