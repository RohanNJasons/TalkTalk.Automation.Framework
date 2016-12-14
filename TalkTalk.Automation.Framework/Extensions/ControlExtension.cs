using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using TalkTalk.Automation.Framework.Constants;

namespace TalkTalk.Automation.Framework.Extensions
{
    /// <summary>
    /// Control extensions are used so that all common tasks are processed the same way. E.G. Inputting text, Clicking a button etc.
    /// </summary>
    public static class ControlExtension
    {
        /// <summary>
        /// Find an element, waiting until a timeout is reached if necessary.
        /// </summary>
        /// <param name="context">The search context.</param>
        /// <param name="by">Method to find elements.</param>
        /// <param name="timeout">How many seconds to wait.</param>
        /// <param name="displayed">Require the element to be displayed?</param>
        /// <returns>The found element.</returns>
        public static IWebElement FindElement(this ISearchContext context, By by, uint timeout, bool displayed = false)
        {
            var wait = new DefaultWait<ISearchContext>(context) {Timeout = TimeSpan.FromSeconds(timeout)};
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            return wait.Until(ctx => {
                var elem = ctx.FindElement(by);
                if (displayed && !elem.Displayed)
                    return null;

                return elem;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0) return driver.FindElement(@by);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(drv => drv.FindElement(@by));
        }

        /// <summary>
        /// Waits for an element to be clickable before attempting to click it
        /// </summary>
        /// <param name="element">The element we wish to interact with</param>
        /// <param name="driver">The web browser driver</param>
        /// <param name="timeoutInSeconds">How long we wish to wait for</param>
        public static void WaitUntilClickable(this IWebElement element, IWebDriver driver, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(ExpectedConditions.ElementToBeClickable(element));
            }
        }

        //This method needs revision as it is not working in the TalkTalk website
        /// <summary>
        /// Waits for a specified length of time for the javascript on a browser to load 
        /// </summary>
        /// <param name="driver">The web browser driver</param>
        /// <returns>The browser once the javascript load has completed</returns>
        public static bool WaitForJavaScriptLoading(IWebDriver driver, int timeoutInSeconds)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));
            return wait.Until(webDriver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readystate").Equals("complete"));
        }

        /// <summary>
        /// Waits the set amount of time before exiting if the control is not found.  Searches every second. This is a test.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="timeoutInSeconds">Timeout to find the control.</param>
        /// <returns>True of false if control is in visible state.</returns>
        public static bool WaitForElementIsVisible(this IWebElement element, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            var result = false;
            var timeout = 0;
            while (result == false)
            {
                if (timeout >= timeoutInSeconds)
                {
                    break;
                }
                Thread.Sleep(Timeouts.OneSecond);
                timeout += 1;
                result = element.Displayed;
            }
            return result;
        }

        /// <summary>
        /// Common method used to enter text the value into the element property.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="value">The text value to be entered in the WebElement.</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        public static void EnterText(this IWebElement element, IWebDriver driver, string value, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            element.SendKeys(value);
        }

        /// <summary>
        /// Common method used to click the element property.
        /// </summary>
        /// <param name="element">The WebElement that is being clicked.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        public static void Click(this IWebElement element, IWebDriver driver, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            element.Click();
            WaitForJavaScriptLoading(driver, timeoutInSeconds);
        }

        /// <summary>
        /// Select the item from a drop down with the given value.
        /// </summary>
        /// <param name="element">The WebElement that is being used to find the text value.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="value">The value the is to be selected in the drop down control.</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        public static void SelectDropDown(this IWebElement element, IWebDriver driver, string value, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            new SelectElement(element).SelectByText(value);
        }

        /// <summary>
        /// Select the check box element if not already selected.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        public static void SelectCheckBox(this IWebElement element, IWebDriver driver, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            if (!element.Selected)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Unselected the check box element if not already un-selected.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        public static void UnselectCheckBox(this IWebElement element, IWebDriver driver, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            if (element.Selected)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Gets the text value from the passed in element.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        /// <returns>The text value that is in on the element.</returns>
        public static string GetText(this IWebElement element, IWebDriver driver, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            WaitForElementIsVisible(element, timeoutInSeconds);
            WaitForJavaScriptLoading(driver, timeoutInSeconds);
            return element.Text ?? String.Empty;
        }

        /// <summary>
        /// Wait for the validation message to be displayed and then return the text in message.
        /// </summary>
        /// <param name="element">The WebElement that is being used.</param>
        /// <param name="timeoutInSeconds">Timeout to find the control</param>
        /// <returns>The text value that is in on the element.</returns>
        public static string GetTextWhenShown(this IWebElement element, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitForElementIsVisible(element, timeoutInSeconds);
            return element.Text ?? string.Empty;
        }

        /// <summary>
        /// Gets a collection of all the rows in the given Table.
        /// </summary>
        /// <param name="element">The WebElement of the Table object that is being used.</param>
        /// <param name="driver">WebDriver to work with</param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns>Collection of WebElements for each row.</returns>
        public static ReadOnlyCollection<IWebElement> GetTableRows(this IWebElement element, IWebDriver driver, int timeoutInSeconds = TimeoutInSeconds.ControlTimeout)
        {
            WaitUntilClickable(element, driver, timeoutInSeconds);
            WaitForElementIsVisible(element, timeoutInSeconds);
            WaitForJavaScriptLoading(driver, timeoutInSeconds);
            return element.FindElements(By.CssSelector("tbody tr"));
        }
    }
}
