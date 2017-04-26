#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");

private Action RestoreNuget(string packageFile)
{
    var nuggetSettings = new NuGetRestoreSettings
    {
        PackagesDirectory = "./packages"
    };

    return () => NuGetRestore(packageFile, nuggetSettings);
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

private Action BuildProject(string targetProject)
{
    var buildSettings = new MSBuildSettings 
    {
        Verbosity = Verbosity.Minimal,
        Configuration = "Release"
    };

    return () => MSBuild(targetProject, buildSettings);
}

//Ultrawave Core
Task("Ultrawave.Nuget")
    .Does(RestoreNuget("./Toggl.Ultrawave/packages.config"));

Task("Ultrawave.Build")
    .IsDependentOn("Ultrawave.Nuget")
    .Does(BuildProject("./Toggl.Ultrawave/Toggl.Ultrawave.csproj"));

//Ultrawave Unit Tests
Task("Ultrawave.Tests.Unit.Nuget")
    .IsDependentOn("Ultrawave.Nuget")
    .Does(RestoreNuget("./Toggl.Ultrawave.Tests/packages.config"));

Task("Ultrawave.Tests.Unit.Build")
    .IsDependentOn("Ultrawave.Tests.Unit.Nuget")
    .Does(BuildProject("./Toggl.Ultrawave.Tests/Toggl.Ultrawave.Tests.csproj"));

Task("Ultrawave.Tests.Unit.Run")
    .IsDependentOn("Ultrawave.Tests.Unit.Build")
    .Does(Test("./bin/Release/*.Ultrawave.Tests.dll"));

//Ultrawave Integration Tests
Task("Ultrawave.Tests.Integration.Nuget")
    .IsDependentOn("Ultrawave.Nuget")
    .Does(RestoreNuget("./Toggl.Ultrawave.Tests.Integration/packages.config"));

Task("Ultrawave.Tests.Integration.Build")
    .IsDependentOn("Ultrawave.Tests.Integration.Nuget")
    .Does(BuildProject("./Toggl.Ultrawave.Tests.Integration/Toggl.Ultrawave.Tests.Integration.csproj"));

Task("Ultrawave.Tests.Integration.Run")
    .IsDependentOn("Ultrawave.Tests.Integration.Build")
    .Does(Test("./bin/Release/*.Ultrawave.Tests.Integration.dll"));

//Ultrawave All Tests
Task("Ultrawave.Tests.Run")
    .IsDependentOn("Ultrawave.Tests.Unit.Run")
    .IsDependentOn("Ultrawave.Tests.Integration.Run");

//Default Operation
Task("Default")
    .IsDependentOn("Ultrawave.Tests.Unit.Run");

RunTarget(target);