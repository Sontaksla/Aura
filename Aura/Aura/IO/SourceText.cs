using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.IO
{
    public class SourceText
    {
        public string Text;
        private SourceText(string text)
        {
            Text = text;
        }
        public static SourceText From(string raw) 
        {
            return new SourceText(raw);
        }
    }
}
