using Aura.Core;
using AuraCli.Context;
using System;
using Aura.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraCli
{
    public sealed class CreateProjectExecutable : ExecutableLine
    {
        protected override ExecutableLine Check(string[] args, params string[] flags)
        {
            if (args.Length == 0) 
            {
                string str = Process.GetCurrentProcess().MainModule.FileName;
                args = new string[] { str.Substring(0, str.LastIndexOf('\\')) };
            } 

            return new CreateProjectExecutable(ctx =>
            {
                if (ctx.Project != null)
                {
                    return "Project is already created, full path: " + ctx.Project.FullPath;
                }
                if (!Directory.Exists(args[0])) 
                {
                    return "No such direcrory found!";
                }
                Helpers.MainDir = args[0];
                ctx.Project = new Project(Helpers.MainDir);
                ctx.Project.AddBranch(new Branch("auramain"));

                ctx.Project.GetBranch("auramain").Commits.Add(new Commit("Initial commit."));

                Helpers.CurrBranch = "auramain";
                return $"Repository created at ##3({args[0]})";
            });
        }
        public CreateProjectExecutable(Func<CurrentContext, string> func) : base(func, CommandType.CreateProject)
        {

        }
        public CreateProjectExecutable() : base(null, CommandType.CreateProject) { }
    }
}
