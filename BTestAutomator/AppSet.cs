using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BTestAutomator
{
    public static class AppSet
    {
        public static IConfigurationBuilder Config() { 
            return new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        } 
        public static string UserName(string user = "csadmin")
        { 
            return AppSet.Profiles().GetSection(user).GetSection("user").Value;
        }
        public static string Pass(string user = "csadmin")
        {
            return AppSet.Profiles().GetSection(user).GetSection("pass").Value;
        }
        public static IConfigurationSection Profiles()
        { 
            return AppSet.Config().Build().GetSection("profiles");
        } 
    }
}
