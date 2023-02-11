using Framework.Logging;
using Framework.Logging.Console.Extensions;
using Framework.Logging.Elastic.Extensions;
using Framework.Logging.Files.Extensions;
using Serilog;
var logBuilder = new LogBuilder()
    .MinimumLevel.Debug()
    .DeleteBefore.Week(2)
    .DateTime.Set(false)
    .WriteTo.File(rollingInterval: Framework.Logging.RollingInterval.Day)
    .WriteTo.ElasticSearch("http://127.0.0.1:9200/", "elastic", "elastic")
    .WriteTo.Console()
    .CreateLogger();

logBuilder.Info("Logger Test", "1", "2", "3");
var logs = logBuilder.Read(DateTime.Now);

foreach (var log in logs)
{
    Console.WriteLine(log);
}

Console.ReadKey();