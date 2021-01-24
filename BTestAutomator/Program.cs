using System;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace BTestAutomator
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var val1 = builder.Build().GetSection("profiles").GetSection("csadmin").GetSection("user").Value;
            var val2 = builder.Build().GetSection("profiles").GetSection("csadmin").GetSection("pass").Value;
            Console.Write($"The values of parameters are: {val1} and {val2}");
        
        }
    }
}
