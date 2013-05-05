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
    EnableDashDash : bool;
    ParsingCulture : CultureInfo;
}

module ArgParser =
    let private getComparer config =
        if config.CaseSensitive then StringComparer.Ordinal
        else StringComparer.OrdinalIgnoreCase
    let private parse<'a> (parseFunc : unit -> ParserResult<'a>, config) =
        let filterUnknown (result : ParserResult<'a>) =
            match config.IgnoreUnknownArguments with
                | true -> ParserResult(result.Tag, result.Value, query { for err in result.Errors do
                                                                            where(err.Tag <> ErrorType.UnknownOptionError)
                                                                            select err }, result.VerbTypes)
                | _ -> result
        let displayHelp result =
            match config.HelpWriter with
                | null -> result
                | _ -> config.HelpWriter.Write(HelpText.AutoBuild result) |> ignore; result
        parseFunc()
            |> filterUnknown
            |> displayHelp
    let private tokenize args optionSpecs config =
        if config.EnableDashDash then
            Tokenizer.PreprocessDashDash(args, Func<seq<string>, StatePair<seq<Token>>>(fun a -> Tokenizer.Tokenize(a, Func<string,bool>(fun name -> NameLookup.Contains(name, optionSpecs,  getComparer config)))))
        else
            Tokenizer.Tokenize(args, Func<string,bool>(fun name -> NameLookup.Contains(name, optionSpecs,  getComparer config)))

    let ParseOptions<'a when 'a : (new : unit -> 'a)> config args =
        parse((fun () -> InstanceBuilder.Build(Func<'a>(fun () -> new 'a()), Func<seq<string>,seq<OptionSpecification>,StatePair<seq<Token>>>(fun ar os -> tokenize ar os config), args, getComparer config, config.ParsingCulture)), config)

    let ParseVerbs config args types =
        parse((fun () -> InstanceChooser.Choose(Func<seq<string>,seq<OptionSpecification>,StatePair<seq<Token>>>(fun ar os -> tokenize ar os config), types, args, getComparer config, config.ParsingCulture)), config)
