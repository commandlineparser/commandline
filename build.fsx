#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing

let buildDir = "./build/"
let testDir = "./build/test/"
let packagingDir = "./nuget/"

let authors = ["Giacomo Stelluti Scala"]
let projectDescription = "The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks."
let projectSummary = "Command Line Parser Library"
let buildVersion = "2.0.0.0"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "Default" (fun _ ->
    trace "Command Line Parser Library 2.0 pre-release"
)

Target "BuildLib" (fun _ ->
    !! "src/CommandLine/CommandLine.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "LibBuild-Output: "
)

Target "BuildTest" (fun _ ->
    !! "src/CommandLine.Tests/CommandLine.Tests.csproj"
        |> MSBuildDebug testDir "Build"
        |> Log "TestBuild-Output: "
)

Target "Test" (fun _ ->
    //trace "Running Tests..."
    !! (testDir @@ "\CommandLine.Tests.dll") 
      |> xUnit2 (fun p -> {p with HtmlOutputPath = Some(testDir @@ "xunit.html")})
)

//Target "Package" (fun _ ->
//    let net40Dir = packagingDir @@ "lib/net40/"
//    CleanDir net40Dir
//    CopyFile net40Dir (buildDir @@ "CommandLine.dll")
//    CopyFile net40Dir (buildDir @@ "CommandLine.dll.XML") 
//    
//    NuGet (fun p -> 
//        {p with
//            Authors = authors
//            Project = "CommandLineParser"
//            Description = projectDescription                               
//            OutputPath = packagingDir
//            Summary = projectSummary
//            WorkingDir = packagingDir
//            Version = buildVersion
//            //AccessKey = myAccesskey
//            Publish = false})    
//            "CommandLine.nuspec"
//)

// Dependencies
"Clean"
    ==> "BuildLib"
    ==> "BuildTest"
    ==> "Test"
    ==> "Default"

RunTargetOrDefault "Default"
