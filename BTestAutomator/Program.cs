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
namespace BTestAutomator
{
    class Program
    {
        static void Main(string[] args)
        {
             
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Type ContextType = Type.GetType($"BTestAutomator.Core.{o.Context.ToString()}, BTestAutomator");
                       IContext context = (IContext)Activator.CreateInstance(ContextType);
                       while (ProcessCommand(o, context));
                       context.Dispose();
                   });
        }
        static bool ProcessCommand(Options o, IContext context) {
            

            Console.WriteLine($"context {string.Join(" | ", Enum.GetNames(typeof(ContextName)))}:");
            var contextIn = Console.ReadLine(); 
            if (!string.IsNullOrEmpty(contextIn) && (o.Context.ToString() != contextIn))
            {
                Type ContextType = Type.GetType($"BTestAutomator.Core.{o.Context.ToString()}, BTestAutomator");
                context = (IContext)Activator.CreateInstance(ContextType);
                o.Context = (ContextName)Enum.Parse(typeof(ContextName), contextIn);
            }
             
            int cnt = 0;
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p =>  typeof(IAutomator).IsAssignableFrom(p) && p.GetCustomAttribute<AutomatorMeta>()?.contextName==o.Context
                ).ToList();
 
            Console.Write($"Automators: {string.Join("", from t in types let counter = cnt++ select $"\n({counter.ToString()}){t}") }\n:");
            string _type = Console.ReadLine();
            if (!Regex.Match(_type, @"^\d+$").Success) return false;

            _type = (from t in types select t.FullName).ElementAt(Convert.ToInt32(_type));
            Console.WriteLine($"{_type}");

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
                Console.WriteLine($"{e.Message}");
            } 
            return true;
        } 
    }
}
