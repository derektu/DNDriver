using System;
using System.Collections;
using System.Collections.Generic;
using log4net;
using CommandLine;

class Program
{
    public class Options
    {
        [Option("server", Required = false, Default = "http://127.0.0.1:4713", HelpText = "define WAD server location")]
        public string server { get; set; }
        
        [Option("path", Required = false, Default = "C:\\Metastock\\Downloader\\Downloader.exe", HelpText = "define full path location of downloader.exe")]
        public string downloaderPath { get; set; }
        
        [Option("user", Required = true, HelpText = "set userId for downloader")]
        public string userId { get; set; }
        
        [Option("password", Required = true, HelpText = "set password for downloader")]
        public string password { get; set; }
        
        [Option("loginRetry", Required = false, Default = 5, HelpText = "try to login that many times")]
        public int loginRetry { get; set; }
        
        [Option("downloadTimeout", Required = false, Default = 30, HelpText = "Wait for download (minutes)")]
        public int downloadTimeout { get; set; }
    }


    public static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
    }
    
    internal static void RunOptions(Options opts)
    {
        var logger = LogManager.GetLogger("App");

        /*
        opts.server = @"http://10.211.55.6:4173";
        opts.downloaderPath = @"C:\Code\Metastock\Downloader\Downloader.exe";
        opts.userId = "2-1765849";
        opts.password = "26BC2A3FAA8A";
        */
        
        log4net.Config.XmlConfigurator.Configure();

        DNDriver dnDriver = new DNDriver(opts.server, opts.downloaderPath, opts.userId, opts.password);
        logger.Info("DNDriver started");
        try
        {
            dnDriver.Connect();
            dnDriver.Download(opts.loginRetry, opts.downloadTimeout);
            logger.Info("DNDriver completed");
            Environment.ExitCode = 0;
        }
        catch (Exception ex)
        {
            logger.Error("DNDriver exception", ex);
            Environment.ExitCode = 1;
        }
        finally
        {
            dnDriver.Disconnect();
        }
    }
}
