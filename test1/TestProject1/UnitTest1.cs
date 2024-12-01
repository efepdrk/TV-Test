using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Android.Enums;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Support.UI;

namespace TestProject1;

public class Tests
{
    private AndroidDriver _driver;
    private WebDriverWait _wait;
    

    [OneTimeSetUp]
    public void SetUp()
    {
        var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
        var driverOptions = new AppiumOptions() {
            AutomationName = AutomationName.AndroidUIAutomator2,
            PlatformName = "Android",
            DeviceName = "emulator-5554",
        };

        // NoReset assumes the app com.google.android is preinstalled on the emulator
        driverOptions.AddAdditionalAppiumOption("noReset", false);
        _driver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(180));
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _wait = new WebDriverWait(_driver,TimeSpan.FromSeconds(30));
    }

    //Called once all tests have been run
    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Dispose();
    }


//------------------------------------------------------------------- TESTS

    [Test]
    // Checks if the "No need, you are already a developer" pop up appears after clicking on android TV OS build
        public void CheckDevOptions()
    {
     OpenApp("com.android.tv.settings","com.android.tv.settings.MainSettings");
     ScrollDownAndClick("System");
     ScrollDownAndClick("About");
     ScrollDownAndClick("Android TV OS build");

     if(IsToastWithTextVisible("No need, you are already a developer"))
     {
        Console.WriteLine("DEV OPTIONS ENABLED");
     } 

    }

    [Test]
    public void Test2()
    {

    }








//------------------------------------------------------------------ CONTROL METHODS
    public void OpenApp(string package,string activity)
    {
        _driver.StartActivity(package,activity);  
    }


    public bool IsToastWithTextVisible(string text)
    {
        try
        {
            IWebElement toast = _wait.Until(driver => driver.FindElement(By.XPath($"//android.widget.Toast[contains(@text, '{text}')]")));
            return toast != null;
        }

        catch(NoSuchElementException)
        {
            Console.WriteLine("TOAST NOT FOUND");
            return false;
        }
        catch(StaleElementReferenceException)
        {
            Console.WriteLine("TOAST NOT FOUND");
            return false;
        }
    }



        public void ScrollDownAndClick(string text)
    {
        bool elementFound = false;

        while(!elementFound)
        {
            IWebElement focusedElement = _driver.FindElement(By.XPath("//*[@focused='true']"));
            IWebElement focusedElementText = focusedElement.FindElement(By.XPath(".//*[string-length(@text) > 0][1]"));
           if(focusedElementText.Text.Contains(text))
           {
                _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_CENTER);
                elementFound = true;
           }
           else 
           {
            Console.WriteLine("FOCUSED ELEMENT: {0}",focusedElement);
            _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_DOWN);
            Thread.Sleep(1000);
           }
        }

    }  




    /*
        for (int i = 0; i < 5; i++)
        {
          _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_DOWN);
          Thread.Sleep(1000);
        }

        IWebElement focusedElement = _driver.FindElement(By.XPath("//*[@focused='true']"));
        Console.WriteLine(focusedElement.Text);
        */
 /*
      IWebElement systemButton = wait.Until(_driver => _driver.FindElement(By.XPath("//*[contains(@text, 'System')]")));
      systemButton.Click();
      
      IWebElement aboutButton = wait.Until(_driver => _driver.FindElement(By.XPath("//*[contains(@text,'About')]")));
      Thread.Sleep(1000);
      aboutButton.Click();
*/

}
