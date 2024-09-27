#!/usr/bin/env powershell
$ErrorActionPreference = "Stop"
$CURRENTPATH=$pwd.Path

function delete_files([string]$path)
{
	If (Test-Path $path){
		Write-Host "Deleting path $path" -ForegroundColor Green
		Remove-Item -recurse -force $path
	}
}

function clean_bin_obj()
{
	cd "$CURRENTPATH"
	# DELETE ALL "BIN" and "OBJ" FOLDERS
	get-childitem -Include bin -Recurse -force | Remove-Item -Force -Recurse
	get-childitem -Include obj -Recurse -force | Remove-Item -Force -Recurse
}

function clean_build()
{
	# DELETE ALL ITEMS IN "BUILD" OUTPUT FOLDER
	Write-Host "Begin clean" -ForegroundColor Green
	if(!(Test-Path "build")){
		mkdir build
	}
	cd build
	Remove-Item * -Recurse -Force
	cd ..
1
	clean_bin_obj
}

function nuget_restore()
{
	#dotnet restore -s "https://www.nuget.org/api/v2;http://gander.dev.townsuite.com/nuget/Procom" --disable-parallel /clp:PerformanceSummary
	NuGet.exe sources add -Name "CI Server" -Source "https://gander.dev.townsuite.com/nuget/Procom" -User "$env:PROGET_USER" -pass "$env:PROGET_PASSWORD"
	nuget restore "TownSuite.CoreDataAccess/TownSuite.CoreDataAccess.sln"
	if ($LastExitCode -ne 0) { throw "nuget failure" }
	cd "$CURRENTPATH"
}

function build()
{
	dotnet build "TownSuite.CoreDataAccess/TownSuite.CoreDataAccess.sln" -p:Configuration="Release"
	if ($LastExitCode -ne 0) { throw "Building solution, TownSuite.CoreDataAccess, failed" }
	
	cd "$CURRENTPATH"
}


function package_nugetpkg(){
	Write-Host "package_nugetpkg" -ForegroundColor Green
	Copy-Item "$CURRENTPATH\TownSuite.CoreDataAccess\TownSuite.CoreDataAccess\bin\Release\*.nupkg" -Destination "$CURRENTPATH/build" -Force
	Copy-Item "$CURRENTPATH\TownSuite.CoreDataAccess\TownSuite.CoreDTOs\bin\Release\*.nupkg" -Destination "$CURRENTPATH/build" -Force
	Copy-Item "$CURRENTPATH\TownSuite.CoreDataAccess\TownSuite.CoreWebAPI\bin\Release\*.nupkg" -Destination "$CURRENTPATH/build" -Force
	delete_files "*.nupkg"
	cd "$CURRENTPATH"
	
}


function get_cpu_count(){
	$cs = Get-WmiObject -class Win32_ComputerSystem
	$cs.numberoflogicalprocessors
}

function build_from_docker(){
	# give 50 percent cpu to this docker build
	$cpuCount = (get_cpu_count)/2
	& "docker" run --memory 4gb --cpus $cpuCount -e PROGET_USER="$env:PROGET_USER" -e PROGET_PASSWORD="$env:PROGET_PASSWORD" --rm -v ${CURRENTPATH}:c:/CoreDataAccess townsuite/dotnet_windows_builder:3.0.2 'cd C:\CoreDataAccess; ls; .\buildrelease.ps1'
	if ( $LastExitCode -eq 99999 ){
		Write-Host "Error in build scripts, exiting..."		
		exit $LastExitCode				
	} 
	
}

if ( ($args.Count -ne 1) -or ($args[0] -eq 'build') )
{
	try{
		clean_build
		nuget_restore
		build
		package_nugetpkg
		clean_bin_obj
	}
	catch{
		Write-Output "Ran into an issue/error: $PSItem"
		clean_bin_obj
		exit 99999
	}
}
elseif ($args[0] -eq 'docker')
{
	# run docker, moutn current direct to docker container c:WebPortals folder and build full
	$cleanpath = $CURRENTPATH.Replace("\", "/").ToLower()
	write-host "path: $cleanpath"
	build_from_docker
}

exit 0