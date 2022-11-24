using Serilog;
using Transpiler;
using Transpiler.Helpers;

var config = ConfigProvider.FromCommandLineArgs(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.File("consoleapp.log")
    .CreateLogger();

Log.Information("Started logging");

var runtime = new RuntimeProcessor();

await runtime.Run(config);
