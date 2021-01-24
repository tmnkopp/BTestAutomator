using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium.Core;

namespace Selenium
{
    public static class Navigate
    {
     
        public static void ToTab(IContext context, string tab) {
            IList<IWebElement> elements; 
            elements = context.Driver.FindElements(By.XPath("//div[@id='ctl00_ContentPlaceHolder1_radTS_Surveys']//li")); 
            foreach (IWebElement element in elements)
                if (element.Text.Contains($"{tab}"))
                {
                    element.Click();
                    break;
                } 
            //  elements = context.Driver.FindElements(By.CssSelector("a[id$='_hl_Launch']"));
            //  if (elements.Count > 0) 
            //       context.Driver.FindElement(By.CssSelector("a[id$='_hl_Launch']")).Click(); 
        }
    }
}
