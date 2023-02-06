using AuraCli.Context;
using Aura.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraCli
{
    public sealed class CommitExecutable : ExecutableLine
    {
        public CommitExecutable(Func<CurrentContext, string> func, CommandType type) : base(func, type)
        {
        }
        public CommitExecutable() : base(null, default) { }
        public static string LogCommit(Commit commit) 
        {
            string[] newFiles = commit.NewFiles.Select(i => $"##10({i.Key})").ToArray();
            string[] modifiedFiles = commit.ModifiedFiles.Select(i => $"##6({i.Key})").ToArray();
            string[] deletedFiles = commit.DeletedFiles.Select(i => $"##4({i})").ToArray();
            return "Changes for commit: \n" +
                        string.Join("\n", newFiles) + "\n" +
                        string.Join("\n", modifiedFiles) + "\n" +
                        string.Join("\n", deletedFiles);
        }
        protected override ExecutableLine Check(string[] args, params string[] flags)
        {
            string flag = flags.FirstOrDefault() ?? "";
            switch(flag) 
            {
                case "-m":
                    return new CommitExecutable(ctx => 
                    {
                        if (args.Length == 1) 
                        {
                            var branch = ctx.Project.GetBranch(Helpers.CurrBranch);
                            if (branch == null) return "Invalid branch";
                            Commit commit = new Commit(args[0]);

                            commit.ModifiedFiles = commit.ModifiedFilesSince(new DirectoryInfo(Helpers.MainDir), branch.Commits[^1], true);
                            commit.NewFiles = commit.NewFilesSince(new DirectoryInfo(Helpers.MainDir), branch.Commits[^1], true);
                            commit.DeletedFiles = commit.DeletedFilesSince(new DirectoryInfo(Helpers.MainDir), branch.Commits[^1], true);

                            branch.Commits.Add(commit);
                            return "commit was added";
                        }

                        return "Maybe you meant: ##3(aura commit -m \"message\")";
                    }, CommandType.CommitCommand);
                case "-i":
                    return new CommitExecutable(ctx =>
                    {
                        var branch = ctx.Project.GetBranch(Helpers.CurrBranch);
                        if (branch == null) return "Invalid branch";
                        int hash = branch.GetCommit(0).GetHashCode();

                        if (args.Length == 1) 
                        {
                            try { hash = Convert.ToInt32(args[0], fromBase: 16); }
                            catch { return "Not a valid hash!"; }
                            Commit commit = branch.GetCommitByHash(hash);
                            if (commit == null) return "Hash does not exist";

                            return LogCommit(commit);
                        }
                        else if (args.Length == 2 && flags.Length == 2 && flags[1] == "-mod") 
                        {
                            try { hash = Convert.ToInt32(args[0], fromBase: 16); }
                            catch { return "Not a valid hash!"; }
                            Commit commit = branch.GetCommitByHash(hash);

                            if (commit.ModifiedFiles.ContainsKey(args[1])) 
                            {
                                return GetCommitModifications(commit, args[1]);
                            }
                            return "Not a modified file!";
                        }
                        return "Maybe you meant: ##3(aura commit -i hash)";
                    }, CommandType.CommitCommand);
                case "-help":
                    return new CommitExecutable(ctx => 
                    {
                        return "\tCommit commands: \n" +
                        "\t\t ##3(aura commit -i hash) => gives info about the commit\n" +
                        "\t\t ##3(aura commit -m message_without_whitespaces) => creates a new commit and adds to the current branch\n" +
                        "\t\t ##3(aura commit -d hash) => deletes ##2(commit)\n" +
                        "\t\t ##3(aura commit -i hash -mod filePath) => gives info about changes made in file during ##2(commit)\n" +
                        "\t Every branch always has one ##2(Initial commit.) commit";
                    }, CommandType.CommitCommand);
            }
            return Unknown("commit");
        }
        private string GetCommitModifications(Commit commit, string modifiedFile) 
        {
            StringBuilder sb = new StringBuilder();
            commit.ModifiedFiles[modifiedFile].OnAdded += (added) => 
            {
                sb.Append($"##2(+line {added.Line}:{added.Text})\n");
            };
            commit.ModifiedFiles[modifiedFile].OnRemoved += (removed) =>
            {
                sb.Append($"##4(-line {removed.Line}:{removed.Text})\n");
            };
            commit.ModifiedFiles[modifiedFile].AnalyzeModification();

            return sb.ToString();
        }
    }
}
