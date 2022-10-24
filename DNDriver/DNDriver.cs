using System;
using System.Diagnostics;
using System.Threading;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

public class DNDriver
{
    private static log4net.ILog logger = LogManager.GetLogger("DN");
    
    // a. Use FindElementByName is much much faster than FindElementByXPath (too slow)
    // b. Use FindElementsXXX other than FindElementXXX to handle not found error
    //
    private static class Locator
    {
        // FindElementByName
        public const string DOWNLOADER_WINDOW = "DownLoader";
            // "//Window[@ClassName=\"Window\"][@Name=\"DownLoader\"]";

        // FindElementByName
        public const string DOWNLOADER_CONSOLE = "DownLoader Console";
            // "//Window[@ClassName=\"Window\"][@Name=\"DownLoader Console\"]";

        // FindElementByName
        public const string DOWNLOAD_BUTTON = "Download...";
            // "//Custom[@ClassName=\"DownloadPanelControl\"]/Button[@ClassName=\"Button\"][@Name=\"Download...\"]/Text[@ClassName=\"TextBlock\"][@Name=\"_Download...\"]";

        // FindElementByName (this is Download tab, another is "Convert" tab   
        public const string DOWNLOAD_TAB = "Download";
            // "//Tab[@ClassName=\"TabControl\"]/TabItem[@ClassName=\"TabItem\"][@Name=\"IMA.Presentation.Downloader.Console.ConsoleTabItem\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Download\"]";

        // FindElementByName            
        public const string LOGIN_WINDOW = "DataLink Login";
            // "//Window[@ClassName=\"Window\"][@Name=\"DataLink Login\"]";

        // FindElementByXPath
        public const string LOGIN_WINDOW_EDIT_USERID = 
            "//Edit[@AutomationId=\"txtUsername\"]";
            //"//Window[@ClassName=\"Window\"][@Name=\"DataLink Login\"]/Edit[@AutomationId=\"txtUsername\"]";
        
        // FindElementByXPath
        public const string LOGIN_WINDOW_EDIT_PASSWORD =
            "//Edit[@AutomationId=\"pwdPassword\"]";
            // "//Window[@ClassName=\"Window\"][@Name=\"DataLink Login\"]/Edit[@AutomationId=\"pwdPassword\"]";

        // FindElementByXPath
        public const string LOGIN_WINDOW_BUTTON_LOGIN = 
            "//Button[@Name=\"Login\"]";
            // "//Window[@ClassName=\"Window\"][@Name=\"DataLink Login\"]/Button[@Name=\"Login\"][@AutomationId=\"btnLogin\"]";

        // FindElementByClassName
        public const string DOWNLOAD_STATUS = "DownloadStatusControl";
            // "//Custom[@ClassName=\"DownloadStatusControl\"]";
            // "//Window[@ClassName=\"Window\"][@Name=\"DownLoader Console\"]//Custom[@ClassName=\"DownloadStatusControl\"]/ProgressBar[@ClassName=\"ProgressBar\"]";
        
        // FindElementByName
        public const string COLLECTION_REPORT_WINDOW = "Collection Report";
        
        // FindElementByName
        public const string COLLECTION_REPORT_CLOSE_BUTTON = "Close";
            // "//Window[@ClassName=\"Window\"][@Name=\"Collection Report\"]/Button[@Name=\"Close\"]";
    }

    private readonly string m_wadServer;
    private readonly string m_appPath;
    private readonly string m_datalinkUserId;
    private readonly string m_datalinkPassword;
    
    private WindowsDriver<WindowsElement> m_session;
    private WindowsElement m_dnWnd;            // Download Window
    private WindowsElement m_dnConsole;        // Downloader Console
    

    /// <summary>
    /// Construct DNDriver, passing necessary information
    /// </summary>
    /// <param name="wadServer">remote WAD server</param>
    /// <param name="appPath">path for the Downloader program</param>
    /// <param name="datalinkUserId">DataLink UserID</param>
    /// <param name="datalinkPassword">DataLink Password</param>
    public DNDriver(string wadServer, string appPath, string datalinkUserId, string datalinkPassword)
    {
        m_wadServer = wadServer;
        m_appPath = appPath;
        m_datalinkUserId = datalinkUserId;
        m_datalinkPassword = datalinkPassword;
    }
    
    /// <summary>
    /// Connect to target server and launch Downloader
    /// <param name="launchTimeout">Timeout(seconds) to launch program</param>
    /// <param name="commandTimeout">Timeout(seconds) for each driver operation</param>
    /// </summary>
    public void Connect(int launchTimeout, int commandTimeout = 60)
    {
        if (m_session != null)
            return;
        
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", m_appPath);
        if (launchTimeout > 0)
            options.AddAdditionalCapability("ms:waitForAppLaunch", launchTimeout.ToString());
        m_session = new WindowsDriver<WindowsElement>(new Uri(m_wadServer), options, TimeSpan.FromSeconds(commandTimeout));
    }

    /// <summary>
    /// Close connection (will close Downloader program)
    /// </summary>
    public void Disconnect()
    {
        if (m_session != null)
        {
            try
            {
                m_session.Close();
            }
            catch (Exception)
            {
                // 
            }
            finally
            {
                m_session = null;
                m_dnWnd = null;
            }
        }
    }

    private WindowsElement FindDownloaderMainWindow()
    {
        Debug.Assert(m_session != null);
        try
        {
            return m_session.FindElementByName(Locator.DOWNLOADER_WINDOW);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindDownloaderConsole()
    {
        Debug.Assert(m_dnWnd != null);
        try
        {
            return (WindowsElement)m_dnWnd.FindElementByName(Locator.DOWNLOADER_CONSOLE);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindDownloadButton()
    {
        Debug.Assert(m_dnConsole != null);
        try
        {
            return (WindowsElement)m_dnConsole.FindElementByName(Locator.DOWNLOAD_BUTTON); 
            
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindDownloadTab()
    {
        Debug.Assert(m_dnConsole != null);
        try
        {
            return (WindowsElement)m_dnConsole.FindElementByName(Locator.DOWNLOAD_TAB); 
            
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindLoginWindow()
    {
        Debug.Assert(m_dnWnd != null);
        try
        {
            return (WindowsElement)m_dnWnd.FindElementByName(Locator.LOGIN_WINDOW);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindLoginUserId(WindowsElement loginWnd)
    {
        try
        {
            return (WindowsElement) loginWnd.FindElementByXPath(Locator.LOGIN_WINDOW_EDIT_USERID);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindLoginPassword(WindowsElement loginWnd)
    {
        try
        {
            return (WindowsElement) loginWnd.FindElementByXPath(Locator.LOGIN_WINDOW_EDIT_PASSWORD);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindLoginLoginButton(WindowsElement loginWnd)
    {
        try
        {
            return (WindowsElement) loginWnd.FindElementByXPath(Locator.LOGIN_WINDOW_BUTTON_LOGIN);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private WindowsElement FindDownloadStatusControl()
    {
        Debug.Assert(m_dnConsole != null);
        try
        {
            return (WindowsElement)m_dnConsole.FindElementByClassName(Locator.DOWNLOAD_STATUS);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    private WindowsElement FindCollectionReportCloseButton()
    {
        Debug.Assert(m_dnWnd != null);
        try
        {
            var wnd = m_dnWnd.FindElementByName(Locator.COLLECTION_REPORT_WINDOW);
            return (WindowsElement)wnd.FindElementByName(Locator.COLLECTION_REPORT_CLOSE_BUTTON);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Let's Download
    /// </summary>
    public void Download(int loginRetry = 5, int downloadTimeout = 30)
    {
        OpenDownloaderConsole();
        
        // TODO: 設定要下載的檔案集
        //
        
        ClickDownloadButton();

        WaitForDownloadToComplete(loginRetry, downloadTimeout);
    }
    
    private void OpenDownloaderConsole()
    {
        var action = "<Open>";
        
        logger.Debug($"{action}#1 finding downloader window...");
        m_dnWnd = FindDownloaderMainWindow();
        if (m_dnWnd == null)
            throw new Exception("Cannot find Downloader main window");
        
        logger.Debug($"{action}#2 finding downloader console...");
        m_dnConsole = FindDownloaderConsole();
        if (m_dnConsole == null)
        {
            logger.Debug($"{action}#3 Click to activate downloader console...");
            // display Downloader Console (send Ctrl+D), and then find again
            //
            m_dnWnd.SendKeys(Keys.Control + "d");

            logger.Debug($"{action}#4 finding downloader console again...");
            m_dnConsole = FindDownloaderConsole();
            if (m_dnConsole == null)
                throw new Exception("Cannot find Downloader Console window");
        }
    }

    private void ClickDownloadButton()
    {
        var action = "<ClickDownload>";

        // Locate Download button
        WindowsElement button = null;
        do
        {
            logger.Debug($"{action}#1 finding Download button...");
            button = FindDownloadButton();
            if (button != null)
                break;
            
            // If not, switch to download tab, and try again
            //
            logger.Debug($"{action}#2 finding Download tab...");
            var downloadTab = FindDownloadTab();
            if (downloadTab == null)
                throw new Exception("Cannot find Download tab icon");
                
            logger.Debug($"{action}#3 switching to Download tab...");
            downloadTab.Click();
            button = FindDownloadButton();
            if (button == null)
                throw new Exception("Cannot find Download button");
            
        } while (false);

        logger.Debug($"{action}#4 clicking Download button...");
        button.Click();
    }

    /// <summary>
    /// Wait for download to complete
    /// </summary>
    /// <param name="loginRetryLimit">If need to login, try this many times</param>
    /// <param name="timeoutInMinutes">wait timeout. Unit is 分鐘(minutes)</param>
    /// <exception cref="Exception"></exception>
    private void WaitForDownloadToComplete(int loginRetryLimit, int timeoutInMinutes)
    {
        var action = "<WaitFor>";
        
        // After click Download button, the following things can happen
        //  - Login window is displayed
        //  - Download Progress window is displayed
        //  - Collection Report window is displayed (when download finish, this will display automatically)
        //
        var loginCount = 0;
        DateTime dtExpire = DateTime.Now.AddMinutes(timeoutInMinutes);
        while (DateTime.Now < dtExpire)
        {
            var reportCloseButton = FindCollectionReportCloseButton();
            if (reportCloseButton != null)
            {
                logger.Debug($"{action}#0 ReportWindow found, try to close it...");
                reportCloseButton.Click();
                return;
            }
            
            var loginWnd = FindLoginWindow();
            if (loginWnd != null)
            {
                logger.Debug($"{action}#1 login window is displayed...");
                if (loginCount >= loginRetryLimit)
                    throw new Exception("Cannot login");
                
                logger.Debug($"{action}#2 try to login...");
                HandleLoginWindow(loginWnd);
                loginCount++;
            }
            else
            {
                var downloadProgress = FindDownloadStatusControl();
                if (downloadProgress != null)
                {
                    logger.Debug($"{action}#3 download in progress...");
                    
                    // TODO: log current progress information
                    /*
                    var texts = downloadProgress.FindElementsByClassName("TextBlock");
                    foreach (var text in texts)
                    {
                        // Look for text that looks like "Completed: <n>" or "Errors: <n>"
                        //
                        Console.WriteLine(text.Text);
                    }
                    */
                }
            }
            Thread.Sleep(1*1000);
        }
        throw new Exception("Download timeout");
    }

    private void HandleLoginWindow(WindowsElement loginWnd)
    {
        var action = "<Login>";
        
        logger.Debug($"{action}#1 finding login controls...");
        var editUserId = FindLoginUserId(loginWnd);
        var editPassword = FindLoginPassword(loginWnd);
        var btnLogin = FindLoginLoginButton(loginWnd);
        
        if (editUserId == null)
            throw new Exception("Cannot find UserId Control");
        
        if (editPassword == null)
            throw new Exception("Cannot find Password Control");

        if (btnLogin == null)
            throw new Exception("Cannot find Login button");
        
        logger.Debug($"{action}#2 set userid...");
        editUserId.Click();
        editUserId.Clear();
        editUserId.SendKeys(m_datalinkUserId);
        
        logger.Debug($"{action}#3 set password...");
        editPassword.Click();
        editPassword.Clear();
        editPassword.SendKeys(m_datalinkPassword);
        
        logger.Debug($"{action}#4 click login button...");
        btnLogin.Click();
    }
}