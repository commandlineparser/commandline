#r "packages/FAKE.3.35.1/tools/FakeLib.dll"
open Fake

let buildDir = "./build/"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Default" (fun _ ->
    trace "TODO: complete"
)

// Dependencies
"Clean"
    ==> "Default"

RunTargetOrDefault "Default"
