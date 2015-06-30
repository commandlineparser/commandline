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

let (|Parsed|Failed|) (r : ParserResult<'a>) =
  if Seq.isEmpty r.Errors then Parsed(r.Value)
  else Failed(r.Errors)

let args = fsi.CommandLineArgs.[1..]
let result = Parser.Default.ParseArguments<options>(args)

match result with
  | Parsed(opts) -> printf "%s" (formatInput opts)
  | Failed(errs) -> printf "Invalid: %A, Errors: %u\n" args (Seq.length errs)
