Imports CommandLine
Imports CommandLine.Text

Public Interface IOptions

    <[Option]("n"c, "lines", SetName:="bylines", [Default]:=5UI, HelpText:="Lines to be printed from the beginning or end of the file.")>
    Property Lines As UInteger?

    <[Option]("c"c, "bytes", SetName:="bybytes", HelpText:="Bytes to be printed from the beginning or end of the file.")>
    Property Bytes As UInteger?

    <[Option]("q"c, "quiet", HelpText:="Supresses summary messages.")>
    Property Quiet As Boolean

    <[Value](0, MetaName:="input file", Required:=True, HelpText:="Input file to be processed.")>
    Property FileName As String

End Interface

<[Verb]("head", HelpText:="Displays first lines of a file.")>
Public Class HeadOptions
    Implements IOptions
    Public Property Lines As UInteger? Implements IOptions.Lines

    Public Property Bytes As UInteger? Implements IOptions.Bytes

    Public Property Quiet As Boolean Implements IOptions.Quiet

    Public Property FileName As String Implements IOptions.FileName

    <Usage(ApplicationAlias:="ReadText.Demo.VB.exe")>
    Public Shared ReadOnly Iterator Property IEnumerable() As IEnumerable(Of Example)
        Get
            Yield New Example("normal scenario", New HeadOptions With {.FileName = "file.bin"})
            Yield New Example("specify bytes", New HeadOptions With {.FileName = "file.bin", .Bytes = 100})
            Yield New Example("supress summary", UnParserSettings.WithGroupSwitchesOnly(), New HeadOptions With {.FileName = "file.bin", .Quiet = True})
            Yield New Example("read more lines", New UnParserSettings() {UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly()}, New HeadOptions With {.FileName = "file.bin", .Lines = 10})
        End Get
    End Property

End Class

<[Verb]("tail", HelpText:="Displays last lines of a file.")>
Public Class TailOptions
    Implements IOptions
    Public Property Lines As UInteger? Implements IOptions.Lines

    Public Property Bytes As UInteger? Implements IOptions.Bytes

    Public Property Quiet As Boolean Implements IOptions.Quiet

    Public Property FileName As String Implements IOptions.FileName

End Class
