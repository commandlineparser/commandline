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
    let private getComparer config =
        if config.CaseSensitive then StringComparer.Ordinal
        else StringComparer.OrdinalIgnoreCase
    let private parse<'a>(parseFunc : unit -> ParserResult<'a>, config) =
        let filterUnknown (result: ParserResult<'a>) =
            match config.IgnoreUnknownArguments with
                | true -> ParserResult(result.Tag, result.Value, query { for err in result.Errors do
                                                                            where(err.Tag <> ErrorType.UnknownOptionError)
                                                                            select err }, result.VerbTypes)
                | _ -> result
        let displayHelp (result: ParserResult<'a>) =
            match config.HelpWriter with
                | null -> result
                | _ -> config.HelpWriter.Write(HelpText.AutoBuild(result)) |> ignore; result
        parseFunc()
            |> filterUnknown
            |> displayHelp

    let ParseOptions<'a when 'a : (new : unit -> 'a)> (config, args) =
        parse((fun () -> InstanceBuilder.Build(Func<'a>(fun () -> new 'a()), args, getComparer(config), config.ParsingCulture)), config)

    let ParseVerbs (config, args, types) =
        parse((fun () -> InstanceChooser.Choose(types, args, getComparer(config), config.ParsingCulture)), config)
