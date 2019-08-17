using NLog;
using NLog.Layouts;

namespace XamariNES.Common.Logging
{
    /// <summary>
    ///     NLog Custom Logger
    ///
    ///     Handy for debugging within the application. Really only implemented
    ///     where I need it while working on a specific portion of XamariNES.
    /// </summary>
    public class CustomLogger : Logger
    {

        static CustomLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
    
            //Setup Console Logging
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
            {
                Layout = Layout.FromString("${shortdate}\t${time}\t${level}\t${callsite}\t${message}")
            };
            config.AddTarget(logconsole);

            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = @"c:\nes\log.txt",
                Layout = Layout.FromString("${shortdate}\t${time}\t${level}\t${callsite}\t${message}"),
                DeleteOldFileOnStartup = true
            };
            config.AddTarget(logfile);
            config.AddRuleForAllLevels(logconsole);
            config.AddRuleForAllLevels(logfile);
            LogManager.Configuration = config;
        }
    }
}
