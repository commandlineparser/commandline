#r "./../src/CommandLine/bin/Debug/CommandLine.dll"

open System
open CommandLine

type options = {
    [<Option(HelpText = "Define a string value here.")>] StringValue : string;
    [<Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")>] IntSequence : seq<int>;
    [<Option('x', HelpText = "Define a boolean or switch value here.")>] BoolValue : bool;
    [<Value(0)>] LongValue : int64 option;
  }

let longOrZero o =
  match o with
    | Some(v) -> v
    | _ -> 0L

let formatInput (o : options)  =
    sprintf "--stringvalue: %s\n-i: %A\n -x: %b\n value: %u\n" o.StringValue (Array.ofSeq o.IntSequence) o.BoolValue (longOrZero o.LongValue)

let args = fsi.CommandLineArgs.[1..]
let parsed = Parser.Default.ParseArguments<options>(args)

if Seq.isEmpty parsed.Errors
then Console.WriteLine(formatInput parsed.Value)
else printf "Invalid: %A\n" args
