/// This is a Cake build script for this project.
/// See: https://cakebuild.net

using System;

var target = Argument("target", "");

if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Debug");
var solutionFile = Argument("solution", "NServiceBus.MongoDB.sln");
var semanticVersion = Argument("semanticVersion", "1.0.0-local");

Task("Default")
    .IsDependentOn("Build");

Task("All")
    .IsDependentOn("CleanAll")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings { Configuration = configuration });
});

Task("Test")
    .Does(() =>
{
    var projectFiles = GetFiles("./src/**/*MongoDB.Tests.csproj");

    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath, new DotNetCoreTestSettings { Configuration = configuration } );
    }
});

Task("Clean")
    .Does(() =>
{
     DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings { Configuration = configuration } );
     
     DeleteDirectories(
         GetDirectories("./src/**/bin")
         .Union(GetDirectories("./src/**/obj")),
          new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Pack")
    .Does(() =>
{
     var packSettings = new DotNetCorePackSettings
     {
         Configuration = configuration,
         MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("Version", semanticVersion)
     };

     DotNetCorePack("./src/NServiceBus.MongoDB", packSettings);
});

Task("CleanAll").IsDependentOn("Clean");

RunTarget(target);