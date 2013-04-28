namespace CommandLine.FSharp

open System
open System.IO
open System.Globalization
open CommandLine.Core
open CommandLine
open CommandLine.Text

type public ParserConfig = {
    CaseSensitive : bool;
    HelpWriter : TextWriter;
    IgnoreUnknownArguments : bool;
    ParsingCulture : CultureInfo;
}

module ArgParser =
    let private handleUnknownArguments<'a> (result : ParserResult<'a>, ignoreUnknownArguments) =
        if ignoreUnknownArguments then
            let errs = query { for err in result.Errors do
                               where(err.Tag <> ErrorType.UnknownOptionError)
                               select err }
            ParserResult(result.Tag, result.Value, errs, result.VerbTypes)
        else result

    let private displayHelp<'a> (result : ParserResult<'a>, helpWriter: TextWriter option) =
        let write (res : ParserResult<'a>, hw : TextWriter) =
            hw.Write(HelpText.AutoBuild(res)) |> ignore
            res
        match helpWriter with
            | Some(helpWriter) -> write(result, helpWriter)
            | _ -> result

    let private makeParserResult<'a> (parseFunc : unit -> ParserResult<'a>, config : ParserConfig) =
        let writerToOption(hw: TextWriter) =
            match hw with
                | null -> None
                | _ -> Some(hw)
        displayHelp(handleUnknownArguments(parseFunc(), config.IgnoreUnknownArguments), writerToOption(config.HelpWriter))

    let private getComparer config =
        if config.CaseSensitive then StringComparer.Ordinal
        else StringComparer.OrdinalIgnoreCase
  
    let ParseOptions<'a when 'a : (new : unit -> 'a)> (config, args) =
        let parseFunc = fun() -> InstanceBuilder.Build(Func<'a>(fun () -> new 'a()), args, getComparer(config), config.ParsingCulture)
        makeParserResult(parseFunc, config)

//    let ParseVerbs args =
//        args