using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;
using BTestAutomator.Core;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace BTestAutomator
{
    class Issue {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string issuekey { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string labels { get; set; }
        public string version { get; set; }
        public string summary { get; set; }
        public string content { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
 
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                Type ContextType = Type.GetType($"BTestAutomator.Core.{o.Context.ToString()}, BTestAutomator");
                IContext context  = (IContext)Activator.CreateInstance(ContextType);
                while (ProcessCommand(o, context));
                context.Dispose();
            });
        }
        static bool ProcessCommand(Options o, IContext context) {

            int cnt = 0;

            Console.Write($"{string.Join("", from t in Enum.GetNames(typeof(ContextName)) let counter = cnt++ select $"\n({counter.ToString()}){t}") }\n:");
            string _context = Console.ReadLine();
            if (!Regex.Match(_context, @"^\d+$").Success) return false;

            if ((ContextName)Convert.ToInt32(_context) != o.Context) {
                o.Context = (ContextName)Convert.ToInt32(_context);
                Type ContextType = Type.GetType($"BTestAutomator.Core.{o.Context.ToString()}, BTestAutomator");
                context = (IContext)Activator.CreateInstance(ContextType);
            }

            cnt = 0;
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p =>  typeof(IAutomator).IsAssignableFrom(p) && p.GetCustomAttribute<AutomatorMeta>()?.contextName==o.Context
                ).ToList();
            
            Console.Write($"Automators: {string.Join("", from t in types let counter = cnt++ select $"\n({counter.ToString()}){t}") }\n:");
            string _type = Console.ReadLine();
            if (!Regex.Match(_type, @"^\d+$").Success) return false;

            _type = (from t in types select t.FullName).ElementAt(Convert.ToInt32(_type)); 

            Type AutomatorType = Type.GetType($"{_type}, {_type.Split(".")[0]}");
            ConstructorInfo[] ctors = AutomatorType.GetConstructors();
            ParameterInfo[] PI = ctors[0].GetParameters();
            List<object> oparms = new List<object>();
            foreach (ParameterInfo parm in PI)  {
                Console.Write($"{parm.Name} ({parm.ParameterType.Name}):");
                var item = Console.ReadLine();
                if (parm.ParameterType.Name.Contains("Int"))
                    oparms.Add(Convert.ToInt32(item));
                else
                    oparms.Add(item);
            } 
            try  { 
                IAutomator obj = (IAutomator)Activator.CreateInstance(AutomatorType, oparms.ToArray());
                obj.Automate(context);
            }  catch (Exception e)  { 
                Console.WriteLine($"{e.Source} {e.Message}");
            } 
            return true;
        } 
    }
}
