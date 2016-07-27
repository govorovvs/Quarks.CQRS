//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var nugetSource = Argument("source", "https://nuget.org/api/v2/");
var nugetApiKey =  Argument("apikey", "");

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

string SOLUTION_DIR = Context.Environment.WorkingDirectory.FullPath;
string SOLUTION_NAME = "Quarks.CQRS.sln";
string SOLUTION_PATH = SOLUTION_DIR + "/" + SOLUTION_NAME;
string TOOLS_DIR = SOLUTION_DIR + "/tools";
string PACKAGES_DIR = SOLUTION_DIR + "/packages";
string SOURCE_DIR = SOLUTION_DIR + "/src";
string TESTS_DIR = SOLUTION_DIR + "/tests";
string ARTIFACTS_DIR = SOLUTION_DIR + "/artifacts";

string NUNIT_EXE_PATH = TOOLS_DIR + "/NUnit.ConsoleRunner/tools/nunit3-console.exe";
string NUGET_EXE_PATH = TOOLS_DIR + "/nuget.exe";

string[] projects = System.IO.Directory.GetDirectories(SOURCE_DIR);
string[] testProjects = System.IO.Directory.GetDirectories(TESTS_DIR);

//////////////////////////////////////////////////////////////////////
// INITIALIZE
//////////////////////////////////////////////////////////////////////

Task("Initialize")
    .Does(() =>
    {
        CreateDirectory(ARTIFACTS_DIR);
    }
);

//////////////////////////////////////////////////////////////////////
// NUGET
//////////////////////////////////////////////////////////////////////

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    }
);

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("Build")
	.IsDependentOn("Restore")
	.Does(() =>
	{
		foreach(var project in projects)
		{
			BuildProject(project);
		}
	}
);

//////////////////////////////////////////////////////////////////////
// UNIT TESTS
//////////////////////////////////////////////////////////////////////

Task("Tests")
	.IsDependentOn("Initialize")
	.IsDependentOn("Build")
	.Does(() =>
	{
		foreach(var project in testProjects)
		{
			BuildProject(project);
		}
		
		string pattern = TESTS_DIR + "/**/" + configuration + "/**/Quarks.CQRS*.Tests.dll";

		RunTests(pattern);
	}
);

//////////////////////////////////////////////////////////////////////
// PACKAGING
//////////////////////////////////////////////////////////////////////

Task("Pack")
	.Does(() =>
	{
		CleanDirectories(PACKAGES_DIR);

		foreach(var project in projects)
		{
			PackProject(project);
		}
	}
);

//////////////////////////////////////////////////////////////////////
// PUBLISHING
//////////////////////////////////////////////////////////////////////

Task("Publish")
	.IsDependentOn("Pack")
	.Does(() =>
	{
		DeleteFiles(PACKAGES_DIR + "/*.symbols.nupkg");

		string[] packages = System.IO.Directory.GetFiles(PACKAGES_DIR, "*.nupkg");

		foreach(var package in packages)
		{
			PublishPackage(package);
		}
	}
);

//////////////////////////////////////////////////////////////////////
// HELPER METHODS 
//////////////////////////////////////////////////////////////////////

void RunTests(string pattern)
{
	NUnit3Settings settings = new NUnit3Settings 
	{
		ToolPath = NUNIT_EXE_PATH,
		Agents = 1,
		Results = ARTIFACTS_DIR + "/unit-tests.xml"
	};

	NUnit3(pattern, settings);
}

void BuildProject(string projectPath)
{
	var settings = new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		Verbose = true,
		NoIncremental = true
	};

	DotNetCoreBuild(projectPath, settings);
}

void PackProject(string projectPath)
{
	var settings = new DotNetCorePackSettings 
	{
		Configuration = configuration,
		Verbose = true,
		OutputDirectory = PACKAGES_DIR
	};

	DotNetCorePack(projectPath, settings);
}

void PublishPackage(string packagePath)
{
	var settings = new NuGetPushSettings 
	{
		Source = nugetSource,
		ApiKey = nugetApiKey,
		ToolPath = NUGET_EXE_PATH
	};

	NuGetPush(packagePath, settings);
}

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);