#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");

var buildSettings = new MSBuildSettings 
{
    Verbosity = Verbosity.Minimal,
    Configuration = "Release"
};

var testSettings = new XUnit2Settings
{
    Parallelism = ParallelismOption.All,
    HtmlReport = true,
    NoAppDomain = true,
    OutputDirectory = "./bin"
};

Task("BuildAll")
    .Does(() => 
    {
        var togglSln = "./Toggl.sln";

        NuGetRestore(togglSln);
        MSBuild(togglSln, buildSettings);
    });

Task("UnitTests")
    .IsDependentOn("BuildAll")
    .Does(() =>
    {
        var testAssemblies = GetFiles("./bin/Release/*.Tests.dll");
        XUnit2(testAssemblies, testSettings);
    });

Task("IntegrationTests")
    .IsDependentOn("BuildAll")
    .Does(() =>
    {
        var testAssemblies = GetFiles("./bin/Release/*.Integration.dll");
        XUnit2(testAssemblies, testSettings);
    });

Task("AllTests")
    .IsDependentOn("UnitTests")
    .IsDependentOn("IntegrationTests");

Task("Default")
    .IsDependentOn("BuildAll");

RunTarget(target);