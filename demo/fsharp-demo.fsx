#r "./../src/CommandLine/bin/Debug/CommandLine.dll"

open CommandLine

type options = {
    [<Option(HelpText = "Define a string value here.")>] stringValue : string;
    [<Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")>] intSequence : seq<int>;
    [<Option('x', HelpText = "Define a boolean or switch value here.")>] boolValue : bool;
    [<Value(0)>] longValue : int64 option;
  }

let formatLong o =
  match o with
    | Some(v) -> string v
    | _ -> "{None}"

let formatInput (o : options)  =
    sprintf "--stringvalue: %s\n-i: %A\n-x: %b\nvalue: %s\n" o.stringValue (Array.ofSeq o.intSequence) o.boolValue (formatLong o.longValue)

let (|Parsed|Failed|) (r : ParserResult<'a>) =
  if Seq.isEmpty r.Errors then Parsed(r.Value)
  else Failed(r.Errors)

let args = fsi.CommandLineArgs.[1..]
let result = Parser.Default.ParseArguments<options>(args)

match result with
  | Parsed(opts) -> printf "%s" (formatInput opts)
  | Failed(errs) -> printf "Invalid: %A, Errors: %u\n" args (Seq.length errs)
