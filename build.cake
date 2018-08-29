/// This is a Cake build script for this project.
/// See: https://cakebuild.net

using System;

var target = string.IsNullOrWhiteSpace(Argument("target", "")) ? "Default" : Argument<string>("target");
var configuration = Argument("configuration", "Debug");
var solutionFile = Argument("solution", "NServiceBus.MongoDB.sln");

Task("Default")
    .IsDependentOn("Build");

Task("All")
    .IsDependentOn("CleanAll")
    .IsDependentOn("UpdateAssemblyInfo")
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

Task("UpdateAssemblyInfo")
    .Does(() =>
{
    ExecuteGitVersionForDotnetCore(updateAssemblyInfo: true);
});

Task("Pack")
    .IsDependentOn("UpdateAssemblyInfo")
    .IsDependentOn("Build")
    .Does(() =>
{
    var semanticVersion = ExecuteGitVersionForDotnetCore();
    
    var packSettings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("Version", semanticVersion.SemVer)
    };

     DotNetCorePack("./src/NServiceBus.MongoDB", packSettings);
});

Task("CleanAll").IsDependentOn("Clean");

RunTarget(target);

// Use the pure .Net Core beta version of the GitVersion command
// See: https://github.com/GitTools/GitVersion/pull/1269
// TODO: Migrate this to run as a dotnet tool and use the builtin Cake GitVersion command.
// See: https://cakebuild.net/dsl/gitversion/
private GitVersion ExecuteGitVersionForDotnetCore(bool updateAssemblyInfo = false)
{
    IEnumerable<string> redirectedStandardOutput;
    IEnumerable<string> redirectedStandardError; 

    try
    {
        #tool nuget:https://ci.appveyor.com/nuget/gitversion-8nigugxjftrw?package=GitVersion.CommandLine.DotNetCore&version=4.0.0-beta0014
        var gitVersionBinaryPath = MakeAbsolute((FilePath)"./tools/GitVersion.CommandLine.DotNetCore.4.0.0-beta0014/tools/GitVersion.dll").ToString();

        var arguments =  new ProcessArgumentBuilder()
            .AppendQuoted(gitVersionBinaryPath)
            .Append("/nofetch");

        if (updateAssemblyInfo)
        {
            arguments.Append("/updateassemblyinfo");
        }

        var exitCode = StartProcess(
            "dotnet",
            new ProcessSettings
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments
            },
            out redirectedStandardOutput,
            out redirectedStandardError);

        if (exitCode != 0)
        {
            var error = string.Join(Environment.NewLine, redirectedStandardError.ToList());
            Error($"GitVersion: exit code: {exitCode} - {error}");
            throw new InvalidOperationException();
        }
    }
    catch (System.Exception ex)
    {
        Error($"Exception {ex.GetType()} - {ex.Message} - {ex.StackTrace} - Has inner exception {ex.InnerException != null}");
        throw;
    }

    #addin nuget:?package=Cake.Json&version=3.0.0
    #addin nuget:?package=Newtonsoft.Json&version=9.0.1
    var json = string.Join(Environment.NewLine, redirectedStandardOutput.ToList());
    return DeserializeJson<GitVersion>(json);
}