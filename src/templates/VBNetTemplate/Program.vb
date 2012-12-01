#Region "License"
''
'' Your Project Name Here: Program.vb
''
'' Author:
''   Your Name Here (insert-your@email.here)
''
'' Copyright (C) 2012 Your Name Here
''
'' [License Body Here]
#End Region
#Region "Using Directives"
Imports CommandLine
Imports CommandLine.Text
#End Region

Friend NotInheritable Class Program
    Private Class Options
        Inherits CommandLineOptionsBase

        <[Option]("t", "text", Required:=True, HelpText:="text value here")>
        Public Property TextValue As String

        <[Option]("n", "numeric", HelpText:="numeric value here")>
        Public Property NumericValue As Double = 0

        <[Option]("b", "bool", HelpText:="on|off switch here")>
        Public Property BooleanValue As Boolean

        <HelpOption()>
        Public Function GetUsage() As String
            Dim help = New HelpText() With {
                .Heading = New HeadingInfo(ThisAssembly.Title, ThisAssembly.InformationalVersion),
                .Copyright = New CopyrightInfo(ThisAssembly.Author, 2012),
                .AdditionalNewLineAfterOption = True,
                .AddDashesToOption = True}
            Me.HandleParsingErrorInHelp(help)
            help.AddPreOptionsLine("<<license details here.>>")
            help.AddPreOptionsLine("Usage: VBNetTemplate -tSomeText --numeric 2012 -b")
            help.AddOptions(Me)

            Return help
        End Function

        Private Sub HandleParsingErrorInHelp(help As HelpText)
            If Me.LastPostParsingState.Errors.Count > 0 Then
                Dim errors = help.RenderParsingErrorsText(Me, 2) ''indent by two spaces
                If Not String.IsNullOrEmpty(errors) Then
                    help.AddPreOptionsLine(String.Concat(Environment.NewLine, "ERROR(S):"))
                    help.AddPreOptionsLine(errors)
                End If
            End If
        End Sub
    End Class

    Shared Sub Main(args As String())
        Dim options = New Options()
        If CommandLineParser.Default.ParseArguments(args, options) Then
            Console.WriteLine("t|ext: " + options.TextValue)
            Console.WriteLine("n|umeric: " & options.NumericValue)
            Console.WriteLine("b|ool: " & options.BooleanValue.ToString.ToLower)
        End If
    End Sub
End Class
