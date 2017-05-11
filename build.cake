#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");

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

private Action BuildSolution(string targetProject)
{
    var buildSettings = new MSBuildSettings 
    {
        Verbosity = Bitrise.IsRunningOnBitrise ? Verbosity.Verbose : Verbosity.Minimal,
        Configuration = "Release"
    };

    return () => MSBuild(targetProject, buildSettings);
}

//Build
Task("Nuget")
    .Does(() => NuGetRestore("./Toggl.sln"));

Task("Build")
    .IsDependentOn("Nuget")
    .Does(BuildSolution("./Toggl.sln"));

//Unit Tests
Task("Tests.Unit")
    .IsDependentOn("Build")
    .Does(Test("./bin/Release/*.Tests.dll"));

//Integration Tests
Task("Tests.Integration")
    .IsDependentOn("Build")
    .Does(Test("./bin/Release/*.Tests.Integration.dll"));

//UI Tests
Task("Tests.UI")
    .IsDependentOn("Build")
    .Does(Test("./bin/Release/*.Tests.UI.dll"));

// All Tests
Task("Tests")
    .IsDependentOn("Tests.Unit")
    .IsDependentOn("Tests.Integration")
    .IsDependentOn("Tests.UI");

//Default Operation
Task("Default")
    .IsDependentOn("Tests.Unit");

RunTarget(target);