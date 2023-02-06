using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Extensions
{
    public static class StringExtension
    {
        public static Dictionary<string, int> GetLines(this string src)
        {
            Dictionary<string, int> lines = new Dictionary<string, int>();
            int ind = 0;
            foreach (var line in src.Split('\n'))
            {
                lines[line] = ind++;

            }
            return lines;
        }
    }
}
