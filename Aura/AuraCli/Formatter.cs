using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuraCli
{
    internal static class Formatter
    {
        /// <summary>
        /// Formats given text with standart output writer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stdout"></param>
        public static void Format(string text) 
        {
            Dictionary<int, (int, int, ConsoleColor)> startIndexColor = new Dictionary<int, (int, int,ConsoleColor)>();
            foreach (Match match in Regex.Matches(text, "##\\d{1,2}\\(.+?\\)")) 
            {
                int asInt = int.Parse(match.Value[2].ToString());
                if (char.IsDigit(match.Value[3]))
                {
                    asInt = int.Parse(match.Value[2].ToString() + match.Value[3].ToString());
                    startIndexColor[match.Index] = (match.Length, 5, (ConsoleColor)asInt);
                }

                else startIndexColor[match.Index] = (match.Length, 4, (ConsoleColor)asInt);
            };
            int length = -1;
            int offset = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (startIndexColor.ContainsKey(i)) 
                {
                    int prev = i;
                    length = startIndexColor[i].Item1;
                    offset = startIndexColor[i].Item2;

                    Console.ForegroundColor = startIndexColor[i].Item3;
                    i += offset;
                    for (; i < prev + length - 1; i++)
                    {
                        Console.Write(text[i]);
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;

                }
                else
                    Console.Write(text[i]);
            }
            Console.WriteLine();
        }
    }
}
