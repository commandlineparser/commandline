#Region "License"
'<copyright file="Options.vb" company="Your name here">
'   Copyright 2013 Your name here
' </copyright>
'
' [License Body Here]
#End Region
#Region "Using Directives"
Imports CommandLine
Imports CommandLine.Text
#End Region

Friend Class Options

    <[Option]("t"c, "text", Required:=True, HelpText:="text value here")>
    Public Property TextValue As String

    <[Option]("n"c, "numeric", HelpText:="numeric value here")>
    Public Property NumericValue As Double = 0

    <[Option]("b"c, "bool", HelpText:="on|off switch here")>
    Public Property BooleanValue As Boolean

    <HelpOption()>
    Public Function GetUsage() As String

        Return HelpText.AutoBuild(Me, Sub(current As HelpText) HelpText.DefaultParsingErrorsHandler(Me, current))

    End Function
End Class