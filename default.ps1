# psake script for NServiceBus.MongoDB

Framework "4.0"
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Properties {
    $baseDir = resolve-path .\.
    $sourceDir = "$baseDir\src"

    $packageDir = "$buildDir\package"
    
    $companyName = "SharkByte Software Inc."
	$projectName = "NServiceBus.MongoDB"
    $solutionName = "NServiceBus.MongoDB"
    $configurations = @("Debug","Release","DebugContracts")
	$projectConfig = $configurations[0]
    

    # if not provided, default to 1.0.0.0
    if(!$version)
    {
        $version = "0.1.0.0"
    }
    # tools
    # change testExecutable as needed, defaults to mstest
    $testExecutable = "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\mstest.exe"
    
    $unitTestProject = "NServiceBus.MongoDB.Tests"
    
    $nugetExecutable = "$sourceDir\.nuget\nuget.exe"
	$nuspecFile = "packaging\nuget\NServiceBus.MongoDB.nuspec"
	$nugetOutDir = "packaging\"
}

# default task
task default -Depends Build

task Compile -Depends RestorePackages {
    Write-Host "Building main solution ($projectConfig)" -ForegroundColor Green
    exec { msbuild /nologo /m /nr:false /v:m /p:Configuration=$projectConfig $sourceDir\$solutionName.sln }
}

task UnitTest {
    Write-Host "Executing unit tests ($projectConfig)"
    
    $unitTestAssembly = "$sourceDir\$unitTestProject\bin\$projectConfig\$unitTestProject.dll"
    exec { & "$testExecutable" /testcontainer:$unitTestAssembly }
}

task Clean {
    Write-Host "Cleaning main solution" -ForegroundColor Green
    foreach ($c in $configurations)
    {
        Write-Host "Cleaning ($c)"
        exec { msbuild /t:Clean /nologo /m /nr:false /v:m /p:Configuration=$c $sourceDir\$solutionName.sln }
    }
}

task CleanAll -Depends Clean {
	Write-Host "Removing nuget packages"
	Remove-Item $sourceDir\packages\* -exclude repositories.config -recurse
	Remove-Item $baseDir\packaging\*.nupkg
    
    Write-Host "Deleting the test directories"
	if (Test-Path $testDir)
    {
		Remove-Item $testDir -recurse -force
	}
}

task RestorePackages {
    exec { & "$nugetExecutable" restore $sourceDir\$solutionName.sln }
}

task All {
    foreach ($config in $configurations)
    {
        Write-Host "invoking for $config"
        Invoke-psake -nologo -properties @{"projectConfig"=$config} Compile
    }
    
    Invoke-psake -nologo UnitTest
}

task Build -depends Compile, UnitTest {}

task NugetPackage {
	exec { & "$nugetExecutable" pack $nuspecFile -OutputDirectory $nugetOutDir }
}

task Analysis {
    Invoke-psake -nologo -properties @{"projectConfig"="DebugAnalysis"} Compile
}

task Contracts {
    Invoke-psake -nologo -properties @{"projectConfig"="DebugContracts"} Compile
}
