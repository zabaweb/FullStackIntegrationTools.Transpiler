using Transpiler;
using Transpiler.Helpers;

var config = ConfigProvider.FromCommandLineArgs(args);

var runtime = new RuntimeProcessor();

await runtime.Run(config);
