using System;
using NUnit.Framework;
using TalkTalk.Automation.Framework.Enums;
using TalkTalk.Automation.Framework.Selenium;

namespace TalkTalk.Automation.Framework.Test_DeleteMeBeforePkging
{
    [TestFixture]
    public class Test : TestBase
    {
        private const string Url = "http://www.somewhereinlondon.co.uk/";


        [SetUp]
        public void SetUp() { }

        [TestCase(WebDriver.Firefox)]
        [TestCase(WebDriver.InternetExplorer)]
        [TestCase(WebDriver.Chrome)]
        public void TestOne(WebDriver webDriver)
        {
            CommonTestSetupLocal(new Uri(Url), true, webDriver);
        }

        [TearDown]
        public void TearDown() { }
    }
}
