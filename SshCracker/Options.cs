using CommandLine;

namespace SshCracker
{
    public class Options
    {
        [Option('u', HelpText = "UserName", Required = true)]
        public string UserName { get; set; }

        [Option('t', HelpText = "No of threads. Recommend 4.", Required = false, Default = 4)]
        public int Threads { get; set; }

        [Option('h', HelpText = "IP address of remote host", Required = true)]
        public string Host { get; set; }

        [Option('v', Required = false)]
        public bool Verbose { get; set; }

        [Value(0, Required = false, Default = "./wordlists/top500.txt",
            HelpText = "Optional password list. If no list is supplied a default list will be used.")]
        public string PasswordList { get; set; }
    }
}