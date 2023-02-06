using Aura.Core;
using AuraCli;
using AuraCli.Context;
using System;
using System.Data;
using System.Diagnostics;
using Aura.IO;
namespace AuraCli 
{
    public class Program 
    {
        public static void Main() 
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                }
            };
            Thread tr = new Thread(() =>
            {
                Analyzer analyzer = new Analyzer();
                CurrentContext context = new CurrentContext();
                while (true)
                {
                    Console.Write("user>");
                    string line = Console.ReadLine();
                    if (line.StartsWith(Helpers.StartKeyword + " "))
                    {
                        try 
                        {
                            Formatter.Format(analyzer.Analyze(line.Substring(Helpers.StartKeyword.Length + 1), context));
                        } catch { }
                    }
                    else
                        process.StandardInput.WriteLine(line);
                }
            });
            process.OutputDataReceived += (o, e) => Console.WriteLine(e.Data);
            process.Start();
            process.BeginOutputReadLine();
            tr.Start();

            process.WaitForExit();
        }
    }
}
