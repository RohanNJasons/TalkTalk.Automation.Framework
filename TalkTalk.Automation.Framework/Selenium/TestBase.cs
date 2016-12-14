using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using TalkTalk.Automation.Framework.Constants;
using TalkTalk.Automation.Framework.Enums;

namespace TalkTalk.Automation.Framework.Selenium
{
    /// <summary>
    /// Base class used for all tests to inherit.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Instance of the WebDriver used during the tests.
        /// </summary>
        protected IWebDriver WebBrowserDriver { get; set; }

        #region LocalWebdriver

        /// <summary>
        /// Start up the WebDriver and navigate to the URL specified.
        /// </summary>
        /// <param name="url">The Url that will be loaded in the web page.</param>
        /// <param name="deleteAllCookies">Should the cookies be deleted before starting the browser.</param>
        /// <param name="webDriver">The webdriver that will be used during the test.</param>
        public void CommonTestSetupLocal(Uri url, bool deleteAllCookies = true, WebDriver webDriver = WebDriver.Firefox)
        {
            switch (webDriver)
            {
                case WebDriver.Firefox:
                    InitialiseFirefoxLocal(url, deleteAllCookies);
                    break;
                case WebDriver.InternetExplorer:
                    InitialiseInternetExplorerLocal(url, deleteAllCookies);
                    break;
                case WebDriver.Chrome:
                    InitialiseChromeLocal(url, deleteAllCookies);
                    break;
            }
        }

        private void InitialiseFirefoxLocal(Uri url, bool deleteAllCookies)
        {
            var firefoxOptions = new FirefoxOptions();

            firefoxOptions.SetPreference("browser.download.folderList", 2);
            firefoxOptions.SetPreference("browser.download.dir", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\");
            firefoxOptions.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/zip");

            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    //The geckodriver will need to be placed here. Update to ensure that this is not required.
                    var driverService = FirefoxDriverService.CreateDefaultService(@"C:\Selenium", "geckodriver.exe");
                    driverService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    driverService.HideCommandPromptWindow = true;
                    driverService.SuppressInitialDiagnosticInformation = true;

                    WebBrowserDriver = new FirefoxDriver(driverService, firefoxOptions, TimeSpan.FromSeconds(60));
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException($"Failed to start Web Browser in timely manner. - {message}");
                    }
                }
            }

            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseInternetExplorerLocal(Uri url, bool deleteAllCookies)
        {
            var internetExplorerOptions = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                InitialBrowserUrl = "about:blank",
                EnableNativeEvents = true,
                EnsureCleanSession = true
            };

            WebBrowserDriver = new InternetExplorerDriver(internetExplorerOptions);

            InitialiseWebDriverLocal(url, deleteAllCookies);
        }

        private void InitialiseChromeLocal(Uri url, bool deleteAllCookies)
        {
            WebBrowserDriver = new ChromeDriver();
            InitialiseWebDriverLocal(url, deleteAllCookies);
        }

        private void InitialiseWebDriverLocal(Uri url, bool deleteAllCookies)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    SetTimeout.PageLoad(WebBrowserDriver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));

                    if (deleteAllCookies)
                    {
                        WebBrowserDriver.Manage().Cookies.DeleteAllCookies();
                    }

                    WebBrowserDriver.Manage().Window.Maximize();
                    WebBrowserDriver.Navigate().GoToUrl(url);
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException(string.Format($"Failed to start Web Browser in timely manner. - {message}"));
                    }
                }
            }
        }

        #endregion

        #region GridWebdriver

        /// <summary>
        /// Start up the Remote WebDriver and navigate to the URL specified.
        /// </summary>
        /// <param name="url">The Url that will be loaded in the web page.</param>
        /// <param name="deleteAllCookies">Should the cookies be deleted before starting the browser.</param>
        /// <param name="webDriver">The webdriver that will be used during the test.</param>
        protected void CommonTestSetup(Uri url, bool deleteAllCookies = true, WebDriver webDriver = WebDriver.Firefox)
        {
            switch (webDriver)
            {
                case WebDriver.InternetExplorer:
                    InitialiseInternetExplorer(url, deleteAllCookies);
                    break;
                case WebDriver.Firefox:
                    InitialiseFirefox(url, deleteAllCookies);
                    break;
                case WebDriver.Chrome:
                    InitialiseChrome(url, deleteAllCookies);
                    break;
            }
        }

        //This does not work yet
        private void InitialiseFirefox(Uri url, bool deleteAllCookies)
        {
            var firefoxProfile = new FirefoxProfile
            {
                AcceptUntrustedCertificates = true,
                EnableNativeEvents = true,
            };
            firefoxProfile.SetPreference("browser.download.folderList", 2);
            firefoxProfile.SetPreference("browser.download.dir", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\");
            firefoxProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/zip");

            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    var capabilities = DesiredCapabilities.Firefox();
                    capabilities.SetCapability("marionette", true);
                    capabilities.SetCapability(CapabilityType.BrowserName, "firefox");
                    capabilities.SetCapability(CapabilityType.Platform, new Platform(PlatformType.Any));
                    capabilities.SetCapability(FirefoxDriver.ProfileCapabilityName, firefoxProfile);
                    WebBrowserDriver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["browserDriver"]), capabilities);
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException($"Failed to start Web Browser in timely manner. - {message}");
                    }
                }
            }
            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseInternetExplorer(Uri url, bool deleteAllCookies)
        {
            var capabilities = DesiredCapabilities.InternetExplorer();
            capabilities.SetCapability(CapabilityType.BrowserName, "internet explorer");
            capabilities.SetCapability(CapabilityType.Platform, new Platform(PlatformType.Windows));
            capabilities.SetCapability("IgnoreZoomLevel", true);
            capabilities.SetCapability("InitialBrowserUrl", "about:blank");
            capabilities.SetCapability("IntroduceInstabilityByIgnoringProtectedModeSettings", true);
            capabilities.SetCapability("EnableNativeEvents", true);
            capabilities.SetCapability("EnsureCleanSession", true);
            WebBrowserDriver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["browserDriver"]), capabilities);
            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseChrome(Uri url, bool deleteAllCookies)
        {
            var capabilities = DesiredCapabilities.Chrome();
            capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
            capabilities.SetCapability(CapabilityType.Platform, new Platform(PlatformType.Windows));
            WebBrowserDriver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["browserDriver"]), capabilities);
            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseWebDriver(Uri url, bool deleteAllCookies)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    SetTimeout.PageLoad(WebBrowserDriver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));

                    if (deleteAllCookies)
                    {
                        WebBrowserDriver.Manage().Cookies.DeleteAllCookies();
                    }

                    WebBrowserDriver.Manage().Window.Maximize();
                    WebBrowserDriver.Manage().Cookies.DeleteAllCookies();
                    WebBrowserDriver.Navigate().GoToUrl(url);
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException(string.Format($"Failed to start Web Browser in timely manner. - {message}"));
                    }
                }
            }
        }

        #endregion

        ///<summary>
        ///Takes screenshot after failed test.
        ///</summary>
        public void TakeScreenshotOnFailure()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                TakeScreenShot();
            }
            WebBrowserDriver.Quit();
        }

        private void TakeScreenShot()
        {
            if (WebBrowserDriver != null)
            {
                try
                {
                    string fileNameBase = $"error_{TestContext.CurrentContext.Test.ID}_{TestContext.CurrentContext.Test.MethodName}_{TestContext.CurrentContext.Test.FullName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

                    var artifactDirectory = Path.Combine(Directory.GetCurrentDirectory(), "screenshot_results", DateTime.Now.ToString("yyyyMMdd"));
                    if (!Directory.Exists(artifactDirectory))
                    {
                        Directory.CreateDirectory(artifactDirectory);
                    }

                    string pageSource = WebBrowserDriver.PageSource;
                    string sourceFilePath = Path.Combine(artifactDirectory, fileNameBase + "_source.html");
                    File.WriteAllText(sourceFilePath, pageSource, Encoding.UTF8);
                    Console.WriteLine("Page source: {0}", new Uri(sourceFilePath));

                    ITakesScreenshot takesScreenshot = WebBrowserDriver as ITakesScreenshot;

                    if (takesScreenshot != null)
                    {
                        var screenshot = takesScreenshot.GetScreenshot();

                        string screenshotFilePath = Path.Combine(artifactDirectory, fileNameBase + "_screenshot.png");

                        screenshot.SaveAsFile(screenshotFilePath, ImageFormat.Png);

                        Console.WriteLine("Screenshot: {0}", new Uri(screenshotFilePath));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while taking screenshot: {0}", ex);
                }
            }
        }
    }
}
