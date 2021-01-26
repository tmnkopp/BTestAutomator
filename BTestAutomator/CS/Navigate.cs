using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using BTestAutomator.Core;

namespace BTestAutomator
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
        }
    }
}
