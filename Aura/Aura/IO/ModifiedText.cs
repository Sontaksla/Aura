using Aura.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.IO
{
    public struct ChangedPiece 
    {
        public ChangedPiece(int line, string txt)
        {
            Line = line;
            Text = txt;
        }
        public string Text;
        public int Line;
    }
    public class ModifiedText
    {
        public readonly SourceText Before;
        public readonly SourceText After;

        public event Action<ChangedPiece>? OnAdded;
        public event Action<ChangedPiece>? OnRemoved;
        private ModifiedText(SourceText before, SourceText after) 
        {
            Before = before;
            After = after;

        }
        public void AnalyzeModification() 
        {
            var before = Before.Text.GetLines();
            var after = After.Text.GetLines();

            foreach (var line in before)
            {
                if (after.ContainsKey(line.Key) == false)
                {
                    OnRemoved?.Invoke(new ChangedPiece(line.Value, line.Key));
                }
            }

            foreach (var line in after)
            {
                if (before.ContainsKey(line.Key) == false) 
                {
                    OnAdded?.Invoke(new ChangedPiece(line.Value, line.Key));
                }
            }
        }
        public static ModifiedText FromText(SourceText before, SourceText after) 
        {
            return new ModifiedText(before, after);
        }
    }
}
