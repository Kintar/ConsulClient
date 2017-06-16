#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OctopusTools"
#addin "Cake.FileHelpers"

/*************************************************************
Change project, product, and description to fit your project
*************************************************************/
var solution = "ConsulClient"; // Your solution name
var project = "ConsulClient"; // Your project name
var product = "Consul"; // The product this project is for
var description = ".Net client for the Consul service registry"; // A description of the product

var version = string.Format("{0}.{1}", FindRegexMatchInFile("VERSION", @"\d+(?:\.\d+)+", System.Text.RegularExpressions.RegexOptions.None), EnvironmentVariable("CI_PIPELINE_ID") ?? "0");
var buildDir = Directory("./build");

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var built = Argument("built", false);
var pushNuget = Argument("pushNuget", false);
var buildOctoPackage = Argument("buildOctoPackage", false);

Task("Clean")
   .WithCriteria(() => !built)
   .Does(() => {
      CleanDirectory(buildDir);
   });

Task("RestoreNuGetPackages")
   .WithCriteria(() => !built)
   .Does(() => {
      NuGetRestore(string.Format("{0}.sln", solution));
   });

Task("CreateAssemblyInfo")
   .WithCriteria(() => !built)
   .Does(() => {
      EnsureDirectoryExists(string.Format("./{0}/Properties", project));
      CreateAssemblyInfo(string.Format("./{0}/Properties/AssemblyInfo.cs", project), new AssemblyInfoSettings {
         Product = product,
         Version = version,
         FileVersion = version,
         Description = description
      });
   });

Task("Build")
   .WithCriteria(() => !built)
   .IsDependentOn("CreateAssemblyInfo")
   .IsDependentOn("Clean")
   .IsDependentOn("RestoreNuGetPackages")
   .Does(() => {
      built = true;
      var settings = new MSBuildSettings();
      settings.SetConfiguration(configuration)
         .WithProperty("outdir", MakeAbsolute(buildDir).ToString())
         .SetPlatformTarget(PlatformTarget.MSIL);

      if(buildOctoPackage){
         settings
         .WithProperty("RunOctoPack", "true")
         .WithProperty("OctoPackPackageVersion", version);
      }
      MSBuild(string.Format("./{0}.sln", solution), settings);
   });

Task("RunUnitTests")
   .IsDependentOn("Build")
   .Does(() =>{
      XUnit2("./build/*.Tests.dll");
   });

Task("PackNuGet")
   .IsDependentOn("Build")
   .Does(() => {
      NuGetPack(string.Format("./{0}/{0}.csproj", project), new NuGetPackSettings{
         OutputDirectory = buildDir,
         Properties = new Dictionary<string,string>{{"OutDir", MakeAbsolute(buildDir).ToString()}}
      });
      if(pushNuget){
         var nuGetPath = configuration == "Release" ? "http://nuget.eftdomain.net/api/" : EnvironmentVariable("USERPROFILE")+"\\NuGet";
         NuGetPush(GetFiles(string.Format("{0}/*.nupkg", buildDir)), new NuGetPushSettings{
            Source = nuGetPath
         });
      }
   });

 Task("Octo-Push")
   .WithCriteria(() => built)
   .Does(() => {
         var files = GetFiles(string.Format("{0}/*.nupkg", buildDir));
         if(files.Count() == 0)
            throw new Exception("Nothing to push.");
         NuGetPush(files, new NuGetPushSettings{
            Source = "http://klondike.eftdomain.net/api/",
			ApiKey = "bebca02d-4193-40fe-9040-0023ea43982f"
         });
   });

Task("Default")
    .IsDependentOn("CreateAssemblyInfo")
    .Does(() =>{
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(string.Format("{0} is configured and ready to build.", solution));
    });

RunTarget(target);
