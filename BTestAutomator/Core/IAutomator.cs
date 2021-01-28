using MongoDB.Bson;
using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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
            context.Click("create_link").Pause()
                .SendKeys("summary", _desc)
                .Click("assign-to-me-trigger");
        }
    } 
    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraList : IAutomator
    {  
        public void Automate(IContext context)
        {   
            foreach (var item in context.Driver.FindElements(By.CssSelector("*[id$='gadget-15990-renderbox'] .summary .issue-link")))
            {
                Console.WriteLine($"{ item.GetAttribute("data-issue-key") } { item.Text }" ); 
            } 
        }
    }
    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraLog: IAutomator
    {
        private string _issuekey = null;
        public JiraLog()   {  }
        public void Automate(IContext context)
        {
            new JiraList().Automate(context);
            Console.Write($":");
            _issuekey = Console.ReadLine(); 
            context.Driver.FindElement(By.CssSelector($"tr[data-issuekey$='{_issuekey}'] .issuekey a")).Click(); 
            System.Threading.Thread.Sleep(500); 
            context.Driver.FindElement(By.CssSelector($"*[id*='opsbar-operations_more']")).Click();
            System.Threading.Thread.Sleep(500);
            context.Driver.FindElement(By.CssSelector($"*[class*='issueaction-log-work'] a")).Click();
            System.Threading.Thread.Sleep(500);
            context.Driver.FindElement(By.CssSelector($"*[id*='log-work-time-logged']")).SendKeys("10m");
            //context.Driver.FindElement(By.CssSelector($"*[id*='log-work-submit']")).Click(); 
        }
    }

    [AutomatorMeta(ContextName: ContextName.JIRA)]
    public class JiraExport : IAutomator
    { 
        public JiraExport() { }
        public void Automate(IContext context)
        { 
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("jira");
            var collection = database.GetCollection<BsonDocument>("issues");

            List<string> issues = new List<string>();  
            foreach (var item in context.Driver.FindElements(By.CssSelector("*[id$='gadget-15990-renderbox'] .summary .issue-link")))
                issues.Add(item.GetAttribute("data-issue-key")); 

            foreach (var _issuekey in issues)
            { 
                try  {
                    context.Driver.Navigate().GoToUrl($"https://dayman.cyber-balance.com/jira/si/jira.issueviews:issue-xml/{_issuekey}/{_issuekey}.xml");
                    var src = context.Driver.PageSource;
                    src = src.Substring(src.IndexOf("<item>"), src.IndexOf("</item>") - src.IndexOf("<item>")) + "</item>";
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(src);
                    var title = xmldoc.SelectSingleNode("//title")?.InnerText.Trim();
                    var labels = xmldoc.SelectSingleNode("//labels")?.InnerText.Trim();
                    var summary = xmldoc.SelectSingleNode("//summary")?.InnerText.Trim();
                    var version = xmldoc.SelectSingleNode("//version")?.InnerText.Trim();
                    var link = xmldoc.SelectSingleNode("//link")?.InnerText.Trim();
                    Console.WriteLine($"{ _issuekey } {title}");
                    var post = new BsonDocument  {
                        {"issuekey" , _issuekey},
                        {"title" , title ?? ""},
                        {"link" , link ?? ""},
                        {"labels" , labels  ?? ""},
                        {"version" , version ?? ""},
                        {"summary" , summary ?? ""},
                        {"content" , src  ?? ""}
                };
                    collection.ReplaceOneAsync(
                        filter: new BsonDocument("issuekey", _issuekey),
                        options: new ReplaceOptions { IsUpsert = true },
                        replacement: post  );
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Source} { e.Message}");
                } 
            } 
        }
    }

    //data-issuekey="CS-7900"
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
    public class AdminEditFinding : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("csadmin"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("button[id$='BtnExpandColumn']")).Click();
            context.Driver.FindElement(By.CssSelector(".rgDetailTable input[id$='_EditButton']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class AdminCQMilestones : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("csadmin")); 
            context.Driver.Navigate().GoToUrl("https://localhost/Reports/CustomQueries.aspx");
            ControlPopulate.RadDDL(context, "ddl_ReportList", "HVA Activities");
            ControlPopulate.RadDDL(context, "ddl_Agency", "Justice");
            ControlPopulate.RadDDL(context, "ddl_Bureau", 1);
            ControlPopulate.RadDDL(context, "ddl_HVA", 1);
            ControlPopulate.RadDDL(context, "ddl_POAM", 1);
            context.Click("ExportScreen_input"); 
        }
    }
    [AutomatorMeta(ContextName: ContextName.CSLocal)]
    public class UserEditPoam : BaseAutomator, IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login("epa"));
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_hl_Launch']")).Click();
            System.Threading.Thread.Sleep(2400);
            context.Driver.FindElement(By.CssSelector("input[id$='_btnEdit']")).Click();
            ControlPopulate.GenericForm(context);
            context.Driver.FindElement(By.CssSelector("input[id$='_btnSave']")).Click();

            SelectElement sections = new SelectElement(context.Driver.FindElement(By.CssSelector("select[id$='ddl_Sections']")));
            sections.SelectByIndex(1);
            context.Driver.FindElement(By.CssSelector("input[id$='AddNewMilestone_input']")).Click();

            //_GECBtnExpandColumn

            ControlPopulate.GenericForm(context);
            try  {
                context.Driver.FindElement(By.CssSelector("input[id$='_PerformInsertButton']")).Click();
            } catch (Exception e) {
                System.Console.WriteLine($"{e.Source} {e.Message}"); 
            }
            Console.WriteLine("add activity  y/n:");
            if (Console.ReadLine() == "n") return;

            sections = new SelectElement(context.Driver.FindElement(By.CssSelector("select[id$='ddl_Sections']")));
            sections.SelectByIndex(1);
            context.Driver.FindElement(By.CssSelector("*[id$='_GECBtnExpandColumn']")).Click();
            context.Driver.FindElement(By.CssSelector("*[id$='_AddNewActivity_input']")).Click();
            ControlPopulate.GenericForm(context);

            Console.WriteLine("add report  y/n:");
            if (Console.ReadLine() == "n") return; 
            sections = new SelectElement(context.Driver.FindElement(By.CssSelector("select[id$='ddl_Sections']")));
            sections.SelectByIndex(2);
            ControlPopulate.GenericForm(context);
        }
    }
}
