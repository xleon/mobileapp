#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var buildAll = Argument("buildall", Bitrise.IsRunningOnBitrise);

private string GetCommitHash()
{   
    IEnumerable<string> redirectedOutput;
    StartProcess("git", new ProcessSettings
    {
        Arguments = "rev-parse HEAD",
        RedirectStandardOutput = true
    }, out redirectedOutput);

    return redirectedOutput.Last();
}

private Action Test(string testFiles)
{
    var testSettings = new XUnit2Settings
    {
        Parallelism = ParallelismOption.All,
        HtmlReport = true,
        NoAppDomain = true,
        OutputDirectory = "./bin"
    };

    return () => XUnit2(GetFiles(testFiles), testSettings);
}

private Action BuildSolution(string targetProject, string configuration = "Release", string platform = "")
{
    var buildSettings = new MSBuildSettings 
    {
        Verbosity = Bitrise.IsRunningOnBitrise ? Verbosity.Verbose : Verbosity.Minimal,
        Configuration = configuration
    };

    if (!string.IsNullOrEmpty(platform))
    {
        buildSettings = buildSettings.WithProperty("Platform", platform);
    }

    return () => MSBuild(targetProject, buildSettings);
}

const string path = "Toggl.Ultrawave.Tests.Integration/Configuration.cs";
var commitHash = GetCommitHash(); 
var filePath = GetFiles(path).Single();

var oldFile = TransformTextFile(filePath).ToString();
var newFile = oldFile.Replace("{CAKE_COMMIT_HASH}", commitHash);

Setup(context => System.IO.File.WriteAllText(path, newFile));
Teardown(context => System.IO.File.WriteAllText(path, oldFile));

//Build
Task("Clean")
    .Does(() => 
        {
            CleanDirectory("./bin");
            CleanDirectories("./**/obj");
        });

Task("Nuget")
    .IsDependentOn("Clean")
    .Does(() => NuGetRestore("./Toggl.sln"));

Task("Build.Tests.All")
    .IsDependentOn("Nuget")
    .Does(BuildSolution("./Toggl.sln", "Debug"));

Task("Build.Tests.Unit")
    .IsDependentOn("Nuget")
    .Does(BuildSolution("./Toggl.sln", "UnitTests"));

Task("Build.Tests.Integration")
    .IsDependentOn("Nuget")
    .Does(BuildSolution("./Toggl.sln", "ApiTests"));

Task("Build.Release.iOS")
    .IsDependentOn("Nuget")
    .Does(BuildSolution("./Toggl.sln", "Release", "iPhone"));

//Unit Tests
Task("Tests.Unit")
    .IsDependentOn(buildAll ? "Build.Tests.All" : "Build.Tests.Unit")
    .Does(Test("./bin/Debug/*.Tests.dll"));

//Integration Tests
Task("Tests.Integration")
    .IsDependentOn(buildAll ? "Build.Tests.All" : "Build.Tests.Integration")
    .Does(Test("./bin/Debug/*.Tests.Integration.dll"));

// All Tests
Task("Tests")
    .IsDependentOn("Tests.Unit")
    .IsDependentOn("Tests.Integration");

//Default Operation
Task("Default")
    .IsDependentOn("Tests.Unit");

RunTarget(target);