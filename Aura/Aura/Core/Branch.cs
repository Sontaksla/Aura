using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Core
{
    public sealed class Branch : BranchBase
    {
        public new List<Commit> Commits { get; init; }
        [JsonConstructor]
        public Branch(string name) : base(name)
        {
            Commits = new List<Commit>();
        }
        public void Merge(Branch another) 
        {
            foreach (var commit in another.Commits)
            {
                Commits.Add(commit);
            }
        }
        public Commit? GetCommit(int index) 
        {
            if (index > Commits.Count || index < 0) return Commits[^1];

            return Commits[index];
        }
        public Commit? GetCommitByHash(int hash)
        {
            return Commits.FirstOrDefault(i => i.GetHashCode() == hash);
        }
        public Commit? GetCommitByHash(byte[] fourBytes)
        {
            return Commits.FirstOrDefault(i => i.GetHashCode() == BitConverter.ToInt32(fourBytes, 0));
        }
        public override string ToString()
        {
            string str = $"##3({Name}): ";
            return str + string.Join("\n\t", Commits.Select((CommitBase commit, int index) => $"{index + 1} commit " + commit.ToString()));
        }
    }
}
