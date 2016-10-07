# psake script for NServiceBus.MongoDB

#Requires -Version 2.0
Set-StrictMode -Version Latest

Framework "4.0"
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Properties {
    $baseDir = resolve-path .\.
    $sourceDir = "$baseDir\src"
    $companyName = "SharkByte Software Inc."
	$projectName = "NServiceBus.MongoDB"
    $solutionName = "NServiceBus.MongoDB"
    $configurations = @("Debug","Release","DebugContracts")
	$projectConfig = $configurations[0]
	$allSolution = "$sourceDir\$solutionName.sln"
    $unitTestProject = "NServiceBus.MongoDB.Tests"
    $nugetExecutable = "$sourceDir\.nuget\nuget.exe"
	$nuspecFile = "packaging\nuget\NServiceBus.MongoDB.nuspec"
	$nugetOutDir = "packaging\"
    $msbuildCommand = "MSBuild.exe"
    $msbuildVerbosity = 'minimal'
}

# default task
task default -Depends Build

task All -Depends BuildDebug, BuildRelease, BuildContracts {}

Task BuildDebug { BuildSolution $allSolution '/property:Configuration=Debug' }

Task BuildRelease { BuildSolution $allSolution '/property:Configuration=Release' }

Task BuildContracts { BuildSolution $allSolution '/property:Configuration=DebugContracts' }

task Build -depends BuildDebug {}

task Package -Depends BuildRelease {
	Remove-Item $baseDir\packaging\*.nupkg
	exec { & "$nugetExecutable" pack $nuspecFile -OutputDirectory $nugetOutDir }
}

Task CleanSolution { BuildSolution $allSolution -Target Clean }

Task Clean -Depends CleanTestResults, CleanCode, CleanStyleCop

Task CleanTestResults {	Remove-Item $sourceDir/../TestResults -Force -Recurse -ErrorAction SilentlyContinue }

Task CleanCode -Depends CleanSolution { Get-ChildItem $sourceDir -Include ('bin', 'obj') -Recurse | Where { !$_.FullName.Contains('dependencies') } | ForEach { try { Remove-Item $_.FullName -Force -Recurse  -ErrorAction SilentlyContinue } catch {} } }

Task CleanStyleCop { Get-ChildItem $sourceDir -Include StyleCop.Cache -Recurse -Force | ForEach { Remove-Item $_.FullName -Force } }

Task CleanAll -Depends Clean

#region Helper Functions
function BuildSolution([ValidateScript( { Test-Path $_ -PathType Leaf } )][string]$solutionFile, [string[]] $msbuildArguments = @(), [string] $target = 'Build')
{
    $private:commonMsbuildArguments = $solutionFile, "/target:$target", "/verbosity:$msbuildVerbosity", '/maxcpucount', '/nodeReuse:false',
        '/fileLogger', '/property:WarningLevel=4;TreatWarningsAsErrors=False'

    $arguments = $commonMsbuildArguments + $msbuildArguments
    Write-Output "$msbuildCommand $arguments"
    Exec { & $msbuildCommand $arguments }
}
#endregion