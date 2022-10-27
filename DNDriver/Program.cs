using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using log4net;
using CommandLine;
using log4net.Repository.Hierarchy;

class CommandOptions
{
    [Option("wad", Required = false, Default = @"C:\Metastock\WAD\WinAppDriver.exe", HelpText = "Full path of WinAppDriver.exe if you like DNDriver to launch WAD automatically")]
    public string wadPath { get; set; }

    [Option("server", Required = false, Default = @"http://127.0.0.1:4723", HelpText = "WAD server location")]
    public string wadServer { get; set; }
        
    [Option("downloader", Required = false, Default = @"C:\Metastock\Downloader\Downloader.exe", HelpText = "Full path of downloader.exe")]
    public string downloaderPath { get; set; }
        
    [Option("user", Required = true, HelpText = "UserId for downloader")]
    public string userId { get; set; }
        
    [Option("password", Required = true, HelpText = "Password for downloader")]
    public string password { get; set; }

    [Option("launchTimeout", Required = false, Default = 30, HelpText = "Timeout(seconds) to launch Downloader.exe")]
    public int launchTimeout { get; set; }
        
    [Option("loginRetry", Required = false, Default = 5, HelpText = "max retry to login downloader")]
    public int loginRetry { get; set; }
        
    [Option("downloadTimeout", Required = false, Default = 30, HelpText = "Timeout(minutes) to wait for download to complete")]
    public int downloadTimeout { get; set; }
}

/// <summary>
/// This is the real main program
/// </summary>
class MainProgram
{
    private Process m_processWAD;
    private DNDriver m_driver;
    private ILog m_logger;

    public MainProgram()
    {
        log4net.Config.XmlConfigurator.Configure();
        m_logger = LogManager.GetLogger("App");
    }
    
    public void Run(CommandOptions opts)
    {
        /*
            opts.server = @"http://10.211.55.6:4723";
            opts.downloaderPath = @"C:\Code\Metastock\Downloader\Downloader.exe";
            opts.userId = "2-1765849";
            opts.password = "26BC2A3FAA8A";
        */
        if (!string.IsNullOrEmpty(opts.wadPath))
        {
            try
            {
                m_processWAD = LaunchWAD(opts.wadPath);
            }
            catch (Exception ex)
            {
                m_logger.Error("Launch WAD exception", ex);
                Environment.ExitCode = 1;
                return;
            }
        }

        m_driver = new DNDriver(opts.wadServer, opts.downloaderPath, opts.userId, opts.password);
        m_logger.Info("DNDriver started");
        try
        {
            m_driver.Connect(opts.launchTimeout);
            m_driver.Download(opts.loginRetry, opts.downloadTimeout);
            m_logger.Info("DNDriver completed");
            Environment.ExitCode = 0;
        }
        catch (Exception ex)
        {
            m_logger.Error("DNDriver exception", ex);
            Environment.ExitCode = 1;
        }
        finally
        {
            m_driver.Disconnect();
            if (m_processWAD != null)
            {
                // Terminate WAD 
                try
                {
                    m_processWAD.Kill();
                    m_processWAD.WaitForExit(5000);
                }
                catch (Exception ex)
                {
                    m_logger.Error("Kill WAD exception", ex);
                }
            }
        }
    }

    /// <summary>
    /// Launch WAD if it is not there.
    /// </summary>
    /// <param name="wadFullPath">Full path for WAD program</param>
    /// <returns>process of WAD if it is launched by this API. Meaning that we need to kill it when we are done.</returns>
    private Process LaunchWAD(string wadFullPath)
    {
        // 如果WAD已經跑起來了, 可能是有人手動執行, 那我們就不需啟動, 直接連那一個WAD就好了
        //
        if (IsProcessRunning(wadFullPath))
            return null;

        var wadLogger = LogManager.GetLogger("WAD");
        
        //* Create your Process
        var process = new Process();
        process.StartInfo.FileName = wadFullPath;
        process.StartInfo.Arguments = "";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        //* Set your output and error (asynchronous) handlers
        process.OutputDataReceived += (obj, e)=>
        {
            wadLogger.Info(e.Data);
        };
        process.ErrorDataReceived += (obj, e)=>
        {
            wadLogger.Error(e.Data);
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        // process.WaitForExit();
        
        return process;
    }

    /// <summary>
    /// Check if this 'executable' is already running 
    /// </summary>
    /// <param name="exeFullPath">Full path name for the process</param>
    /// <returns></returns>
    private bool IsProcessRunning(string exeFullPath)
    {
        var filePath = Path.GetDirectoryName(exeFullPath);
        var fileName = Path.GetFileNameWithoutExtension(exeFullPath).ToLower();
        var pList = Process.GetProcessesByName(fileName);
        foreach (var p in pList) 
        {
            try
            {
                if (p.MainModule.FileName.StartsWith(filePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                m_logger.Error("Process checking exception", ex);
            }
        }
        return false;
    }

    private void DebugIdle(int seconds)
    {
        var ev = new ManualResetEvent(false);
        ev.WaitOne(seconds * 1000);
    }
    
}

class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandOptions>(args).WithParsed(RunOptions);
    }

    private static void RunOptions(CommandOptions opts)
    {
        new MainProgram().Run(opts);
    }
    
}
