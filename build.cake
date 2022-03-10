#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"
#addin nuget:?package=Cake.Docker

///////////////////////////////////////////////////
///					Variables					///
///////////////////////////////////////////////////

var target = Argument("target", "Report");
var configuration = Argument("configuration", "Release");
var solution = "MeAgendaAi.sln";
var dirTestResults = "testResults";
var dirCoverage = $"{dirTestResults}\\coverage";

///////////////////////////////////////////////////
///					TASKS						///
///////////////////////////////////////////////////


Task("Docker")
	.Description("Builds the docker image")
	.Does(() => {
		DockerComposeUp(new DockerComposeUpSettings
		{
			DetachedMode = true,
			ProjectDirectory = ".",
			ArgumentCustomization = argument => argument.Append("--build")
		});
	});

Task("Build")
	.Description("Builds the solution")
	.IsDependentOn("Docker")
	.Does(() =>
	{
		DotNetBuild(solution);

	});

Task("Test")
	.Description("Runs the tests")
	.IsDependentOn("Build")
	.Does(() =>
	{
		// Delete folder TestResults
		if (System.IO.Directory.Exists(dirTestResults))		
			System.IO.Directory.Delete(dirTestResults, true);

		// Create settings		
		var settings = new DotNetTestSettings 
		{
			NoRestore = true,
			ArgumentCustomization = argument => argument.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=opencover"),
			ResultsDirectory = "TestResults",
			Verbosity = DotNetCoreVerbosity.Minimal
		};

		DotNetTest(solution, settings);
	});	

Task("Coverage")
	.Description("Generates the code coverage report")
	.IsDependentOn("Test")
	.Does(() => 
	{
		// Delete folder TestResults/Coverage		
		if (System.IO.Directory.Exists(dirCoverage))		
			System.IO.Directory.Delete(dirCoverage, true);

		GlobPattern coverageFiles = "./test/**/*.opencover.xml"; 

		ReportGenerator(coverageFiles, $"./{dirCoverage}", new ReportGeneratorSettings 
		{ 
			ReportTypes = new [] 
			{
				ReportGeneratorReportType.TextSummary,
				ReportGeneratorReportType.HtmlInline_AzurePipelines_Dark
			}
		});
	});

Task("Report")	
	.Description("Open the code coverage report")
	.IsDependentOn("Coverage")
	.Does(() => 
	{	
		if (IsRunningOnWindows())
        {
			var reportFilePath = $"{ System.IO.Directory.GetCurrentDirectory() }\\{ dirCoverage }\\index.html";
			Console.WriteLine($"Opening report file: {reportFilePath}...");                      
            StartProcess("explorer", reportFilePath);
        }

	});

RunTarget(target);


