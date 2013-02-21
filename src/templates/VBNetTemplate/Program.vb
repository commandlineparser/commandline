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

Friend NotInheritable Class Program

    Shared Sub Main(args As String())

        Dim options = New Options()

        If Not CommandLine.Parser.Default.ParseArguments(args, options) Then
            Environment.Exit(CommandLine.Parser.DefaultExitCodeFail)
        End If

        Console.WriteLine("t|ext: " + options.TextValue)
        Console.WriteLine("n|umeric: " & options.NumericValue)
        Console.WriteLine("b|ool: " & options.BooleanValue.ToString.ToLower)

    End Sub
End Class