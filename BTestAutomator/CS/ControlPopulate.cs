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
    public static class ControlPopulate
    {
        public static void GenericForm(IContext context)
        {
            IList<IWebElement> inputs;

            inputs = context.Driver.FindElements(By.CssSelector("input[id*='Numeric']"));
            foreach (IWebElement input in inputs)  {
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys("0");
            }
            inputs = context.Driver.FindElements(By.CssSelector("input[id*='CBPercentage']")); 
            foreach (IWebElement input in inputs)
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys("50");

            inputs = context.Driver.FindElements(By.CssSelector("textarea"));
            foreach (IWebElement input in inputs) {
                if (input.Text=="" )
                    input.SendKeys(input.GetAttribute("name"));
            }
            inputs = context.Driver.FindElements(By.CssSelector("input[id*='date']"));
            foreach (IWebElement input in inputs)
            {
                Random rnd = new Random(); 
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden")
                    input.SendKeys($"12/{rnd.Next(2, 27).ToString()}/2020");
            }
            inputs = context.Driver.FindElements(By.CssSelector("input[type='text']"));
            foreach (IWebElement input in inputs) {
                if (input.GetAttribute("value") == "" && input.GetAttribute("type") != "hidden") {
                    try  {
                        input.SendKeys(input.TagName);
                    } catch (Exception e)    {
                        Console.Write($"{e.Source} {e.Message}"); 
                    }
                }    
            }
            
            inputs = context.Driver.FindElements(By.CssSelector(".RadDropDownList_Default"));
            foreach (IWebElement input in inputs)
            {
                System.Threading.Thread.Sleep(200); 
                try  {
                    input.Click();
                }  catch (Exception e)  {
                    Console.Write($"{e.Source} {e.Message}");
                    return;
                }
                IList<IWebElement> rddlItems = context.Driver.FindElements(By.CssSelector($"#{input.GetAttribute("id")}_DropDown .rddlItem"));
                if (rddlItems.Count > 1)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(1, rddlItems.Count-1);
                    try   {
                        rddlItems[index].Click();
                    }   catch (Exception e)  {
                        Console.Write($"{index.ToString()}");
                        Console.Write($"{e.Source} {e.Message}");
                    } 
                } 
            }
            //inputs = context.Driver.FindElements(By.CssSelector(".RadRadioButtonList_Default .RadRadioButton:first-child"));
            //foreach (IWebElement input in inputs) 
            //    input.Click();  
 
        }
        public static void Form(ChromeDriver driver)
        {
            IWebElement edit = driver.FindElement(By.XPath("//input[contains( @id , '_btnEdit')]"));
            edit.Click();
            System.Threading.Thread.Sleep(100);  
            IList<IWebElement> CBNumerics = driver.FindElements(By.XPath("//input[contains(@id, 'CBNumeric')]"));
            foreach (IWebElement input in CBNumerics)
                Type(input, "0");
            IList<IWebElement> CBPercentages = driver.FindElements(By.XPath("//input[contains(@id, 'CBPercentage')]"));
            foreach (IWebElement input in CBPercentages)
                Type(input, "50"); 
            IList<IWebElement> frequency = driver.FindElements(By.XPath("//input[contains(@id, 'CBFrequency1_tb_frequency')]"));
            foreach (IWebElement input in frequency)
                Type(input, "5");  
            IList<IWebElement> range = driver.FindElements(By.XPath("//select[contains(@id, '_CBFrequency1_tb_range')]"));
            foreach (IWebElement input in range)
                Select(input, 1); 
            System.Threading.Thread.Sleep(100);
            IWebElement save = driver.FindElement(By.XPath("//input[contains( @id , '_btnSave')]"));
            save.Click();
            System.Threading.Thread.Sleep(200);

        }
         public static void Grid(ChromeDriver driver) {
            IList<IWebElement> elements;
            elements = driver.FindElements(By.XPath("//input[contains(@id, '_EditButton')]"));
            if (elements != null)
            {
                while (elements.Count > 0)
                {
                    IWebElement element = elements[0];
                    if (element == null)
                        break;
                    element.Click();
                    IList<IWebElement> inputs = driver.FindElements(By.XPath("//input[contains(@class, 'riTextBox')]"));
                    foreach (IWebElement input in inputs)
                        Type(input, "0");

                    IWebElement sub = driver.FindElement(By.XPath("//input[contains(@id, 'UpdateButton')]"));// 
                    sub.Click(); 
                    System.Threading.Thread.Sleep(200);

                    By by = By.XPath("//a[contains(text(), 'Submit')]");
                    if (Exists(driver, by))
                    {
                        sub = driver.FindElement(by);
                        sub.Click(); 
                        var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                        var alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                        alert.Accept();
                    } 

                    System.Threading.Thread.Sleep(200);
                    elements = driver.FindElements(By.XPath("//input[contains(@id, '_EditButton')]"));
                    if (elements == null)
                        break;
                }
            }
        }
        private static bool Exists(ChromeDriver driver, By by) {
            return driver.FindElements(by).Count > 0; 
        }
        private static void Select(IWebElement input, int key)
        {
            SelectElement sections = new SelectElement(input);
            try
            {
                sections.SelectByIndex(key);
            }
            catch (Exception)
            {
                Console.Write($"{input} {input.Text} {input.TagName}  not valid");
            }
        }

        private static void Type(IWebElement input, string key) {
            try
            {
                input.Clear();
                input.SendKeys(key);
            }
            catch (Exception)
            {
                Console.Write($"{input} {input.Text} {input.TagName}  not valid");
            }
        }
    }
}
