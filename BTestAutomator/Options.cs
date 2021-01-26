using BTestAutomator.Core;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTestAutomator
{
    public class Options
    {
        [Option('c', "Context Name", Required=true )]
        public ContextName Context { get; set; }
        [Option('t', "type")]
        public string Type { get; set; }
        [Option('p', "Profile")]
        public string Profile { get; set; }
        [Option('m', "Message")]
        public string Message { get; set; }
        [Option('v', "Verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
 
    }
}
