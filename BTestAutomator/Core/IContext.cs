using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
namespace Selenium.Core
{
    public struct Login {
        public string UserName { get; set; }
        public string Password { get; set; } 
    }
    public interface IContext{
        public ChromeDriver Driver { get; }
        public void Dispose();
        public void Connect(Login login);
    }  
    public class CSLocalContext : IContext
    {
        private ChromeDriver driver; 
        public ChromeDriver Driver => driver; 
        public CSLocalContext(ChromeDriver Driver)
        {
            driver = Driver; 
        }
        public void Dispose()
        {
            driver.Quit();
        } 
        public void Connect(Login login)
        {
            if (login.UserName == null)
                login.UserName = ""; 

            driver.Navigate().GoToUrl("https://localhost/");  
            driver.FindElementById("Login1_UserName").SendKeys(login.UserName);
            driver.FindElementById("Login1_Password").SendKeys("");
            driver.FindElementByXPath("//input[@name='Login1$LoginButton']").Click(); 
            driver.FindElementByXPath("//input[@name='ctl00$ContentPlaceHolder1$btn_Accept']").Click();
        } 
    }
}
