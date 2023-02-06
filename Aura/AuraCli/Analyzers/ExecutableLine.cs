using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aura.Core;
using AuraCli.Context;

namespace AuraCli
{
    public enum CommandType : byte
    {
        //by default
        NotValid = 0x00,

        //simple types
        Help,
        Info,
        Save,
        Load,
        Log,

        //executes with arguments
        BranchCommand,
        CreateProject,
        CommitCommand,
    }
    public class ExecutableLine
    {
        public readonly Func<CurrentContext, string> Execute;
        public readonly CommandType Type;
        protected ExecutableLine(Func<CurrentContext, string> func, CommandType type) 
        {
            Execute = func;
            Type = type;
        }
        private ExecutableLine() 
        {

        }
        public static ExecutableLine From(string line) 
        {
            string[] args = Regex.Matches(line, " [^ -]*").Select(i => i.Value.Substring(1)).Where(i => !string.IsNullOrEmpty(i)).ToArray();
            string[] flags = Regex.Matches(line, "-\\w+").Select(i => i.Value).ToArray();

            if (Helpers.MainDir == string.Empty)
            {
                return new CreateProjectExecutable().Check(args, flags);
            }

            switch(line) 
            {
                case "save":
                    return new ExecutableLine(ctx => 
                    {
                        ctx.Save();
                        return $"##5(Saved into: {Helpers.MainDir})";
                    }, CommandType.Save);
                case "load":
                    return new ExecutableLine(ctx =>
                    {
                        ctx.Project = CurrentContext.GetCurrentContext().Project;
                        return $"##5(Successfully loaded)";
                    }, CommandType.Load);
                case "help":
                    return new ExecutableLine(ctx =>
                    {
                        return "Aura is a version control system inspired by git. \n" +
                        "There are branches and commits as in git. You can merge, add, remove or check them. \n" +
                        "##6(Main Commands are predefined in README file, recommended to use as a flag), example:\n" +
                        "\t##2(aura branch -help)";
                    }, CommandType.Help);
                case "info":
                    return new ExecutableLine(ctx =>
                    {
                        var branches = ctx.Project.GetBranches().ToArray();
                        return $"This project has {branches.Length} branches and " +
                        $"{branches.Select(i => i.Commits.Count).Sum()} commits total\n" +
                        $"Current branch: {Helpers.CurrBranch}";
                    }, CommandType.Info);
                case "log":
                    return new ExecutableLine(ctx => 
                    {
                        var lastCommit = ctx.Project.Branches[Helpers.CurrBranch].GetCommit(int.MaxValue);
                        Commit curr = new Commit();

                        curr.NewFiles = curr.NewFilesSince(new DirectoryInfo(Helpers.MainDir), lastCommit, false);
                        curr.ModifiedFiles = curr.ModifiedFilesSince(new DirectoryInfo(Helpers.MainDir), lastCommit, false);
                        curr.DeletedFiles = curr.DeletedFilesSince(new DirectoryInfo(Helpers.MainDir), lastCommit, false);

                        return CommitExecutable.LogCommit(curr);
                    }, CommandType.Log);
            }

            if (line.StartsWith("branch ")) 
            {
                return new BranchExecutable().Check(args, flags);
            }
            else if (line.StartsWith("commit ")) 
            {
                return new CommitExecutable().Check(args, flags);
            }

            return new ExecutableLine().Check(null, null);
        }
        protected virtual ExecutableLine Check(string[] args, params string[] flags) 
        {
            return Unknown("");
        }
        protected ExecutableLine Unknown(string include) 
        {
            return new ExecutableLine(ctx =>
            {
                return $"##4(Unknown command!) For more info: \n##3(aura {include} [-]help)";
            }, CommandType.NotValid);
        }
    }
}
