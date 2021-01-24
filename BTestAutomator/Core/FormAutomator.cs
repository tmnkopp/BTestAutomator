using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace Selenium.Core
{

    public abstract class BaseElementSetter {
        public virtual void Set(IWebElement input, string key) {
            try { 
                input.Clear();
                input.SendKeys(key);
            }
            catch (Exception) {
                Console.Write($"{input} {input.Text} {input.TagName}  invalid");
            }
        }
    } 
}
