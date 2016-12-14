using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using TalkTalk.Automation.Framework.Constants;

namespace TalkTalk.Automation.Framework.Selenium
{
    /// <summary>
    /// Base Page that all test pages will inherit to gain access to the Web Driver 
    /// </summary>
    public class BasePage
    {
        /// <summary>
        /// IWebDriver Instancec
        /// </summary>
        public static IWebDriver Driver { get; set; }

        /// <summary>
        /// Initialise all the controls on the page so that they are accessible.
        /// </summary>
        /// <param name="webDriver">WebDriver that is in use.</param>
        protected BasePage(IWebDriver webDriver)
        {
            if (WaitForPageToFinishLoading(webDriver) == false)
            {
                throw new Exception("Timed out while waiting for the page to load.");
            }

            PageFactory.InitElements(webDriver, this);
        }

        /// <summary>
        /// Initialise all the controls on the page so that they are accessible.
        /// </summary>
        /// <param name="webDriver">WebDriver that is in use.</param>
        /// <param name="elementId">The element to initially check to ensure on correct page.</param>
        protected BasePage(IWebDriver webDriver, string elementId)
        {
            if (WaitForPageToFinishLoading(webDriver) == false)
            {
                throw new Exception("Timed out while waiting for the page to load.");
            }

            if (AssertElementIsDisplayed(elementId))
            {
                PageFactory.InitElements(webDriver, this);
            }
        }

        /// <summary>
        /// Wrapped the wait for javascript method for use when loading webpages
        /// </summary>
        /// <param name="webDriver">The web browser driver</param>
        /// <returns>The browser once the javascript load has completed</returns>
        private bool WaitForPageToFinishLoading(IWebDriver webDriver)
        {
            Driver = webDriver;

            IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));
            return wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <summary>
        /// Checks to see if the correct page has been loaded
        /// </summary>
        /// <param name="elementId">the element that is checked to ensure that the correct page is loaded</param>
        /// <returns>Returns a true or false</returns>
        private bool AssertElementIsDisplayed(string elementId)
        {
            const int upper = TimeoutInSeconds.DefaultTimeout;
            for (var i = 0; i < upper; i++)
            {
                try
                {
                    Driver.FindElement(By.Id(elementId));
                    return true;
                }
                catch (NoSuchElementException exception)  {  }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            throw new Exception($"Could not find element with ID - {elementId} on page");
        }
    }
}
