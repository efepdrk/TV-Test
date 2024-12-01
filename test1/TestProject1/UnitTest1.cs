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
        driverOptions.AddAdditionalAppiumOption("noReset", true);
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

    [Test] // Test 1: Confirm that the developer options are on. 
    
        public void CheckDevOptions()
    {
     OpenApp("com.android.tv.settings","com.android.tv.settings.MainSettings");
     ScrollDownToText("System",true);
     ScrollDownToText("About",true);
     ScrollDownToText("Android TV OS build",true);
     bool visible = IsToastWithTextVisible("No need, you are already a developer");

     if(visible)
     {
        Console.WriteLine("DEV OPTIONS ENABLED");
     } 
     else
     {
        Console.WriteLine("NO POP UP");
     }

    }

    [Test] //Test 2 confirm that bold text is off , if it isn't turn it off.
    public void BoldCheck()
    {
        OpenApp("com.android.tv.settings","com.android.tv.settings.MainSettings");    
        ScrollDownToText("Accessibility",true);

        IWebElement boldBox = ScrollDownToText("Bold text",false);
        IWebElement boldCheck = boldBox.FindElement(By.XPath("//*[contains(@checkable, 'true')]"));
        if(boldCheck.GetAttribute("checked") == "true")
        {
            _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_CENTER);
        }
        else
        {
            Console.WriteLine("BOLD TEXT NOT ENABLED");
        }
        
    }


//------------------------------------------------------------------ CONTROL METHODS
    //Not really neccessary, I just didn't want to type _driver.StartActivity every time.
    public void OpenApp(string package,string activity)
    {
        _driver.StartActivity(package,activity);  
    }


    public bool IsToastWithTextVisible(string text)
    {
        WebDriverWait shortWait = new WebDriverWait(_driver,TimeSpan.FromSeconds(5));
        try
        {
            IWebElement toast = shortWait.Until(driver => driver.FindElement(By.XPath($"//android.widget.Toast[contains(@text, '{text}')]")));
            return toast != null;
        }
        catch(NoSuchElementException)
        {
            return false;
        }
        catch(StaleElementReferenceException)
        { 
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            return false; 
        }
    }


        //Takes a string text and bool click and scrolls down until it finds it and presses DPAD Center if specified to do so.
        public IWebElement ScrollDownToText(string text,bool click)
    {
        bool elementFound = false;

        while(!elementFound)
        {
            IWebElement focusedElement = _driver.FindElement(By.XPath("//*[@focused='true']"));
            IWebElement focusedElementText = focusedElement.FindElement(By.XPath(".//*[string-length(@text) > 0][1]"));
           if(focusedElementText.Text.Contains(text))
           {
                if(click)
                {
                    _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_CENTER);
                }
                
                elementFound = true;
                return focusedElement;
           }
           else 
           {
            //Console.WriteLine("FOCUSED ELEMENT: {0}",focusedElement);
            _driver.PressKeyCode(AndroidKeyCode.Keycode_DPAD_DOWN);
            Thread.Sleep(1000);
           }
        }
        return _driver.FindElement(By.XPath("//*[@focused='true']"));
    }  

    
}
