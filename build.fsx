#r "packages/FAKE.3.35.1/tools/FakeLib.dll"
open Fake

//RestorePackages()

let buildDir = "./build/"
let testDir = "./build/test/"

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
    trace "Running Tests..."
    !! (testDir + @"\CommandLine.Tests.dll") 
      |> xUnit (fun p -> {p with OutputDir = testDir})
)

// Dependencies
"Clean"
    ==> "BuildLib"
    ==> "BuildTest"
    ==> "Test"
    ==> "Default"

RunTargetOrDefault "Default"
