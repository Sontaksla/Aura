using Aura.Core;
using AuraCli.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraCli
{
    public sealed class BranchExecutable : ExecutableLine
    {
        protected override ExecutableLine Check(string[] args, params string[] flags)
        {
            if (args.Length > 2)
                return new BranchExecutable(ctx => "branch only cound have 1 or 2 arguments!");


            if (flags.Length == 0 && args.Length == 1)
            {
                return new BranchExecutable(ctx =>
                {
                    if (ctx.Project.GetBranch(args[0]) == null)
                    {
                        return $"No branch found!";
                    }
                    Helpers.CurrBranch = args[0];

                    return $"Switched to ##2({args[0]})";
                });
            }
            string flag = flags.First();
            switch (flag)
            {
                case "-n":
                    return new BranchExecutable(ctx =>
                    {
                        //removing whitespace character
                        string branchName = args[0];
                        ctx.Project.AddBranch(new Branch(branchName));
                        ctx.Project.GetBranch(branchName).Commits.Add(new Commit("Initial commit."));
                        return $"New branch ##2({branchName}) was added!";
                    });
                case "-i":
                    return new BranchExecutable(ctx =>
                    {
                        return ctx.Project.GetBranch(Helpers.CurrBranch).ToString();
                    });
                case "-m":
                    return new BranchExecutable(ctx => 
                    {
                        if (args.Length != 2) return "Maybe you meant: ##3(aura branch -m first secod)";
                        else if (!ctx.Project.Branches.ContainsKey(args[0]) || !ctx.Project.Branches.ContainsKey(args[1]))
                            return "Unknown branch to merge";
                        var br1 = ctx.Project.GetBranch(args[0]);

                        ctx.Project.GetBranch(args[1]).Merge(br1);
                        ctx.Project.DeleteBranch(args[0]);
                        return "Merged";
                    });
                case "-l":
                    return new BranchExecutable(ctx =>
                    {
                        return string.Join("  \n", ctx.Project.GetBranches().Select(br =>
                            br.Name == Helpers.CurrBranch ? $"##2({br.Name})" : br.Name)
                        );
                    });
                case "-d":
                    return new BranchExecutable(ctx =>
                    {
                        ctx.Project.DeleteBranch(args[0]);
                        Helpers.CurrBranch = "auramain";
                        return $"##4(branch {args[0]} was deleted) (if actual)";
                    });
                case "-help":
                    return new BranchExecutable(ctx =>
                    {
                        return "\tBranch commands: \n" +
                        "\t\t ##3(aura branch -n myBranch) => creates a new branch ##2(myBranch)\n" +
                        "\t\t ##3(aura branch -i) => gives info about current branch\n" +
                        "\t\t ##3(aura branch -l) => lists all the branches in the project\n" +
                        "\t\t ##3(aura branch myBranch) => change your branch to ##2(myBranch)\n" +
                        "\t\t ##3(aura branch -d myBranch) => deletes ##2(myBranch) and sends you to ##2(auramain) branch\n" +
                        "\t Main branch is always ##2(auramain).";
                    });

            }

            return Unknown("branch");
        }
        public BranchExecutable(Func<CurrentContext, string> func) : base(func, CommandType.BranchCommand)
        {

        }
        public BranchExecutable() : base(null, CommandType.BranchCommand) { }
    }
}
