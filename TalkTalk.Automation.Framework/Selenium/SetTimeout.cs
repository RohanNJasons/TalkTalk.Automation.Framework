using OpenQA.Selenium;
using System;

namespace TalkTalk.Automation.Framework.Selenium
{
    public class SetTimeout
    {
        /// <summary>
        /// Set the Implicit Wait Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void ImplicitlyWait(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().ImplicitlyWait(timeSpan);
        }

        /// <summary>
        /// Set the Page Load Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void PageLoad(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(timeSpan);
        }

        /// <summary>
        /// Set the Script Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void Script(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().SetScriptTimeout(timeSpan);
        }
    }
}
