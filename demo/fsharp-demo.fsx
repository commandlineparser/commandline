#r "./../src/CommandLine/bin/Debug/CommandLine.dll"

open CommandLine
open CommandLine.Text

type options = {
    [<Option(HelpText = "Input a string value here.", Default="中文")>] stringValue : string;
    [<Option('i', Min = 3, Max = 4, HelpText = "Input a int sequence here.")>] intSequence : int seq;
    [<Option('x', HelpText = "Define a switch (boolean) here.")>] boolValue : bool;
    [<Value(0, MetaName = "longvalue", HelpText = "A long scalar here.")>] longValue : int64 option; }
    with
        [<Usage(ApplicationAlias = "fsi fsharp-demo.fsx")>]
        static member examples
            with get() = seq {
               yield Example("Supply some values", {stringValue = "hello"; boolValue = true; intSequence = seq {1..3}; longValue = Some 10L }) }

let formatLong o =
  match o with
    | Some(v) -> string v
    | _ -> "{None}"

let formatInput (o : options)  =
    sprintf "--stringvalue: %s\n-i: %A\n-x: %b\nvalue: %s\n" o.stringValue o.intSequence o.boolValue (formatLong o.longValue)

let inline (|Success|Fail|) (result : ParserResult<'a>) =
  match result with
  | :? Parsed<'a> as parsed -> Success(parsed.Value)
  | :? NotParsed<'a> as notParsed -> Fail(notParsed.Errors)
  | _ -> failwith "invalid parser result"

let args = fsi.CommandLineArgs.[1..]
let result = Parser.Default.ParseArguments<options>(args)

match result with
  | Success(opts) -> printf "%s" (formatInput opts)
  | Fail(errs) -> printf "Invalid: %A, Errors: %u\n" args (Seq.length errs)
