using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace BTestAutomator.Core
{
    public class Login {
        public Login(string ProfileID)
        {
            this.ProfileID = ProfileID; 
        }
        public string ProfileID { get; set; }
        private string _username;
        public string UserName
        {
            get { return _username ?? AppSet.UserName(ProfileID); }
            set { _username = value; }
        }
        private string _password; 
        public string Password
        {
            get { return _password ?? AppSet.Pass(ProfileID);   }
            set { _password = value; }
        } 
    }
    public enum ContextName
    { 
        CSLocal,  JIRA
    }
    public interface IContext{
        public ChromeDriver Driver { get; }
        public bool Connected { get ; set; }
        public void Dispose();
        public void Connect(Login login);
        public BaseContext SendKeys(string Element, string Content);
        public BaseContext Click(string Element );
        public BaseContext Pause(int Time);
    }
    public abstract class BaseContext {
        protected ChromeDriver driver;
        private bool _connected;

        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        public ChromeDriver Driver  { 
            get {
                if (driver == null) {
                    ChromeOptions options = new ChromeOptions();
                    var chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    chromeDriverService.HideCommandPromptWindow = true;
                    chromeDriverService.SuppressInitialDiagnosticInformation = true;
                    options.AddArgument("log-level=3");
                    driver = new ChromeDriver(chromeDriverService, options); 
                }
             return driver; 
            } 
        }
        public BaseContext()
        {

        }
        public BaseContext Pause(int Time=2500)
        {
            System.Threading.Thread.Sleep(Time);
            return this; 
        }
        public BaseContext SendKeys(string Element, string Content)
        {
            IWebElement element = Driver.FindElement(By.CssSelector($"*[id$='{Element}']"));
            if (element == null)
                element = Driver.FindElement(By.CssSelector($"*[id*='{Element}']")); 
            if (element == null)
                element = Driver.FindElement(By.CssSelector($"*[class*='{Element}']"));
            if (element != null)
                element.SendKeys(Content);
            return this;
        }
        public BaseContext Click(string Element)
        {
            IWebElement element = Driver.FindElement(By.CssSelector($"*[id$='{Element}']"));
            if (element == null)
                element = Driver.FindElement(By.CssSelector($"*[id*='{Element}']"));
            if (element == null)
                element = Driver.FindElement(By.CssSelector($"*[class*='{Element}']"));
            if (element != null)
                element.Click();
            return this;
        }
    }
    public class CSLocal : BaseContext, IContext
    { 
        public CSLocal() : base()
        { 
        } 
        public void Connect(Login login)
        {
            if (!Connected)
            {
                login = login ?? new Login("agency"); 
                Driver.Navigate().GoToUrl("https://localhost/");
                Driver.FindElementById("Login1_UserName").SendKeys(login.UserName);
                Driver.FindElementById("Login1_Password").SendKeys(login.Password);
                Driver.FindElementByXPath("//input[@name='Login1$LoginButton']").Click();
                Driver.FindElementByXPath("//input[@name='ctl00$ContentPlaceHolder1$btn_Accept']").Click();
                Connected = true;
            }  else {
                Driver.Navigate().GoToUrl("https://localhost/ReporterHome.aspx");
            }
        }
        public void Dispose()
        {
            Driver.Quit();
        }
    }    
    public class JIRA : BaseContext, IContext
    { 
        public JIRA(): base()
        {
            Connect(new Login("dayman"));
        } 
        public void Connect(Login login)
        {
            if (!Connected)  {
                Driver.Navigate().GoToUrl("https://dayman.cyber-balance.com/jira/login.jsp");
                Driver.FindElement(By.CssSelector("*[id$='login-form-username']")).SendKeys(login.UserName);
                Driver.FindElement(By.CssSelector("*[id$='login-form-password']")).SendKeys(login.Password);
                Driver.FindElement(By.CssSelector("*[id$='login-form-submit']")).Click();
                Connected = true;
            } else {
                Driver.Navigate().GoToUrl("https://dayman.cyber-balance.com/jira/secure/Dashboard.jspa?selectPageId=12340"); 
            } 
        }
        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
