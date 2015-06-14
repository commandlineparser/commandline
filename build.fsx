#r "packages/FAKE.3.35.1/tools/FakeLib.dll"
open Fake

let buildDir = "./build/"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Default" (fun _ ->
    trace "Command Line Parser Library 2.0 pre-release"
)

Target "BuildLib"(fun _ ->
    !! "src/**/*.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

// Dependencies
"Clean"
    ==> "BuildLib"
    ==> "Default"

RunTargetOrDefault "Default"
