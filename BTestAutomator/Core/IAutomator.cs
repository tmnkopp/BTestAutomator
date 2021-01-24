using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Selenium.Core
{
    public interface IAutomator
    { 
        void Automate(IContext context);
    }
    public class AdminAddPoam : IAutomator
    { 
        public void Automate(IContext context)
        {
            context.Connect(new Login { UserName = "" });
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("button[id$='BtnExpandColumn']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='cmdAddNewPOAM_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    public class AdminAddAssm : IAutomator
    {
        public void Automate(IContext context)
        { 
            context.Connect(new Login { UserName = "" });
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='_cmdAddNewAssessment_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }
    public class UserEditAssm : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login { UserName = "" });
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_lnkAddAssm']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='_EditButton']")).Click();
            context.Driver.FindElement(By.CssSelector("*[id$='_UpdateButton']")).Click();
        }
    }     
    public class UserAddAssmt : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login { UserName = "" });
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='_lnkAddAssm']")).Click();
            context.Driver.FindElement(By.CssSelector("*[id$='cmdAddNewAssessment_input']")).Click();
            ControlPopulate.GenericForm(context);
        }
    }    
    public class AdminEditAssm : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login { UserName = "" });
            Navigate.ToTab(context, "BOD 18-02 Remediation Plans");
            context.Driver.FindElement(By.CssSelector("a[id$='ctl20_lnkAdmin']")).Click();
            context.Driver.FindElement(By.CssSelector("input[id$='ctl04_EditButton']")).Click();
        }
    }    
    public class UserEditPoam : IAutomator
    {
        public void Automate(IContext context)
        {
            context.Connect(new Login { UserName = "" });
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
