using Framework.Logging;
using Framework.Logging.Seq.Extensions;

var logBuilder = new LogBuilder()
    .MinimumLevel.Debug()
    .DeleteBefore.Week(2)
    .DateTime.Set(false)
    .WriteTo.Seq("http://localhost:5341/", "Z70FNmCxu2ZPKc25ov19")
    //.WriteTo.File()
    .CreateLogger();

//await logBuilder.Error("seq test", "1", "2", "3");
var logs = await logBuilder.Read(DateTime.Now);

foreach (var log in logs)
{
    Console.WriteLine(log);
}

Console.ReadKey();