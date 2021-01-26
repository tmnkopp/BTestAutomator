using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTestAutomator.Core
{
    public interface IAutomator
    { 
        void Automate(IContext context);
    }
    public abstract class BaseAutomator {
        public BaseAutomator()
        { 
        }
    }
    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraCreate : IAutomator
    {
        private string _desc = null;
        public JiraCreate(string Description)
        {
            _desc = (string.IsNullOrEmpty(Description)) ? "CS Issue": Description;
        }
        public void Automate(IContext context)
        {
            context.Connect(new Login("dayman"));
            context.Click("create_link").Pause()
                .SendKeys("summary", _desc)
                .Click("assign-to-me-trigger");
        }
    }
    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraConnect : IAutomator
    {
        public JiraConnect()
        {
        }
        public void Automate(IContext context)
        {
            context.Connect(new Login("dayman")); 
        }
    }

    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraLog : IAutomator
    { 
        public JiraLog( )
        { 
        }
        public void Automate(IContext context)
        { 
            context.Driver.Navigate().GoToUrl("https://dayman.cyber-balance.com/jira/secure/Dashboard.jspa?selectPageId=12340");
            Dictionary<string, string> issues = new Dictionary<string, string>();
            foreach (var item in context.Driver.FindElements(By.CssSelector("*[id$='gadget-15990-renderbox'] .issue-link")))
            {
                Console.Write(item ); 
            }

        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class AdminUsers : IAutomator
    {
        private string _username = null;
        public AdminUsers(string EditUser)
        {
            _username = (string.IsNullOrEmpty(EditUser)) ? AppSet.UserName("agency") : EditUser;
        }
        public void Automate(IContext context)
        { 
            context.Connect(new Login("csadmin"));
            context.Driver.Navigate().GoToUrl("https://localhost/UserAccessNew/SelectUser.aspx");
            context.Driver.FindElement(By.CssSelector("*[id$='_WebTextEdit1']")).SendKeys(_username);
            context.Driver.FindElement(By.CssSelector("*[id$='_btn_Run']")).Click(); 
            context.Driver.FindElement(By.CssSelector("*[id$='_link_UserID']")).Click();  
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class AdminAddPoam : IAutomator
    { 
        public void Automate(IContext context)
        {
            context.Connect(new Login("csadmin"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("button[id$='BtnExpandColumn']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='cmdAddNewPOAM_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class AdminAddAssm : BaseAutomator, IAutomator
    { 
        public void Automate(IContext context)
        {
            context.Connect(new Login("csadmin"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='_cmdAddNewAssessment_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class Agency : BaseAutomator, IAutomator
    { 
        private string _tab = null;
        public Agency(string Tab)
        {
            _tab = (string.IsNullOrEmpty(Tab)) ? "BOD" : Tab;
        }
        public void Automate(IContext context)
        {
            context.Connect(new Login("agency"));
            Navigate.ToTab(context, _tab);
            context.Driver.ExecuteChromeCommand("Show Console", null); 
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class CIOPopulate : BaseAutomator, IAutomator
    { 
        public CIOPopulate( )
        { 
        }
        public void Automate(IContext context)
        {
            context.Connect(new Login("agency"));
            Navigate.ToTab(context, "FISMA Quarterly CIO 2021 Q1");
            context.Driver.FindElement(By.CssSelector("*[id$='_hl_Launch']")).Click();
            ControlPopulate.Grid(context.Driver);
            ControlPopulate.Form(context.Driver);
            int[] tabs = new int[] { 1, 3, 5, 6, 8, 9, 10 };
            SelectElement sections; 
            foreach (int tab in tabs)
            {
                sections = new SelectElement(context.Driver.FindElement(By.XPath("//select[contains( @id , 'ctl00_ddl_Sections')]")));
                sections.SelectByIndex(tab);
                ControlPopulate.Form(context.Driver);
            } 
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class UserEditAssm : BaseAutomator, IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("doj"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_lnkAddAssm']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='_EditButton']")).Click();
            context.Driver.FindElement(By.CssSelector("*[id$='_UpdateButton']")).Click();
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class UserAddAssmt : BaseAutomator, IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("doj"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_lnkAddAssm']")).Click();
            context.Driver.FindElement(By.CssSelector("*[id$='cmdAddNewAssessment_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class AdminEditAssm : BaseAutomator, IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("csadmin"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='ctl04_EditButton']")).Click();
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class UserEditPoam : BaseAutomator, IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("doj"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_hl_Launch']")).Click();
            System.Threading.Thread.Sleep(2400);
            context.Driver.FindElement(By.CssSelector("input[id$='_btnEdit']")).Click();
            ControlPopulate.GenericForm(context);
            context.Driver.FindElement(By.CssSelector("input[id$='_btnSave']")).Click();

            SelectElement sections = new SelectElement(context.Driver.FindElement(By.CssSelector("select[id$='ddl_Sections']")));
            sections.SelectByIndex(1);
            context.Driver.FindElement(By.CssSelector("input[id$='AddNewMilestone_input']")).Click();

            ControlPopulate.GenericForm(context);
            try  {
                context.Driver.FindElement(By.CssSelector("input[id$='_PerformInsertButton']")).Click();
            } catch (Exception e) {
                System.Console.WriteLine($"{e.Source} {e.Message}");
            } 
        }
    }
}
