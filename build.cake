var target = Argument("target", "Default");
var runtimes = new[] { "win-x64", "win-x86" };

Task("Clean")
    .Does(() => 
{
    Information("Cleaning output and artifacts...");
    CleanDirectory("./.artifacts");
    CleanDirectories("./**/bin");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
{
    foreach (var runtime in runtimes)
    {
        Information("Building {0}...", runtime);
        DotNetCorePublish("AlphaBeta/AlphaBeta.csproj", new DotNetCorePublishSettings() {
            Configuration = "Release",
            Runtime = runtime,
            Verbosity = DotNetCoreVerbosity.Minimal,
        });
    }
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() => 
{
    foreach (var runtime in runtimes)
    {
        var output = $"./.artifacts/AlphaBeta-{runtime}.zip";
        Information("Packaging {0}...", output);

        var folder = $"./AlphaBeta/bin/Release/net5.0-windows/{runtime}/publish";
        var files = GetFiles($"{folder}/**/*.*");
        Zip(folder, output, files);
    }
});

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);