#r "./../src/CommandLine/bin/Debug/CommandLine.dll"

open CommandLine

type options = {
    [<Option(HelpText = "Define a string value here.")>] StringValue : string;
    [<Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")>] IntSequence : seq<int>;
    [<Option('x', HelpText = "Define a boolean or switch value here.")>] BoolValue : bool;
    [<Value(0)>] LongValue : int64 option;
  }

let formatLong o =
  match o with
    | Some(v) -> string v
    | _ -> "{None}"

let formatInput (o : options)  =
    sprintf "--stringvalue: %s\n-i: %A\n-x: %b\nvalue: %s\n" o.StringValue (Array.ofSeq o.IntSequence) o.BoolValue (formatLong o.LongValue)

let args = fsi.CommandLineArgs.[1..]
let parsed = Parser.Default.ParseArguments<options>(args)

if Seq.isEmpty parsed.Errors
then printf "%s" (formatInput parsed.Value)
else printf "Invalid: %A\n" args
