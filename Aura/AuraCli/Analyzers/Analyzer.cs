using Aura.Core;
using AuraCli.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraCli
{
    public class Analyzer
    {
        public string Analyze(string line, CurrentContext ctx) 
        {
            var executableLine = ExecutableLine.From(line);

            return executableLine.Execute(ctx);
        }
    }
}
