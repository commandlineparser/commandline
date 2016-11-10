Imports System
Imports System.IO
Imports System.Linq
Imports System.Text
Imports CommandLine

Module Program

    Sub Main(ByVal sArgs() As String)

    End Sub

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

End Module
