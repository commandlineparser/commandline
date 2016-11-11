Imports System.IO
Imports System.Text
Imports CommandLine

Module Program

    Function Main(ByVal sArgs() As String) As Integer

        Dim reader As Func(Of IOptions, String) = Function(opts)
                                                      Dim fromTop = opts.[GetType]() = GetType(HeadOptions)
                                                      Return If(opts.Lines.HasValue, ReadLines(opts.FileName, fromTop, CInt(opts.Lines)), ReadBytes(opts.FileName, fromTop, CInt(opts.Bytes)))
                                                  End Function

        Dim header As Func(Of IOptions, String) = Function(opts)
                                                      If opts.Quiet Then Return String.Empty

                                                      Dim fromTop = opts.[GetType]() = GetType(HeadOptions)
                                                      Dim builder = New StringBuilder("Reading ")
                                                      builder = If(opts.Lines.HasValue, builder.Append(opts.Lines).Append(" lines"), builder.Append(opts.Bytes).Append(" bytes"))
                                                      builder = If(fromTop, builder.Append(" from top:"), builder.Append(" from bottom:"))
                                                      Return builder.ToString()

                                                  End Function

        Dim printIfNotEmpty As Action(Of String) = Sub(text)
                                                       If text.Length = 0 Then Return
                                                       Console.WriteLine(text)
                                                   End Sub

        Dim result = Parser.Default.ParseArguments(Of HeadOptions, TailOptions)(sArgs)

        Dim texts = result.MapResult(
                        Function(opts As HeadOptions) Tuple.Create(header(opts), reader(opts)),
                        Function(opts As TailOptions) Tuple.Create(header(opts), reader(opts)),
                        Function() MakeError())

        printIfNotEmpty(texts.Item1)
        printIfNotEmpty(texts.Item2)

        Return If(texts.Equals(MakeError()), 1, 0)

    End Function

    Private Function ReadLines(fileName As String, fromTop As Boolean, count As Integer) As String

        Dim lines = File.ReadAllLines(fileName)
        If (fromTop) Then
            Return String.Join(Environment.NewLine, lines.Take(count))
        End If

        Return String.Join(Environment.NewLine, lines.Reverse().Take(count))

    End Function

    Private Function ReadBytes(fileName As String, fromTop As Boolean, count As Integer) As String

        Dim bytes = File.ReadAllBytes(fileName)
        If (fromTop) Then
            Return Encoding.UTF8.GetString(bytes, 0, count)
        End If

        Return Encoding.UTF8.GetString(bytes, bytes.Length - count, count)

    End Function

    Private Function MakeError() As Tuple(Of String, String)

        Return Tuple.Create(vbNullChar, vbNullChar)

    End Function

End Module
