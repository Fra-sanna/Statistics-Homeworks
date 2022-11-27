Imports Microsoft.VisualBasic.FileIO

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using parser As TextFieldParser = New TextFieldParser("C:\Users\fraga\OneDrive\Documenti\GitHub\Statistics-Homeworks\Homeworks 2\student_statistics.csv.csv")
            parser.TextFieldType = FieldType.Delimited
            parser.SetDelimiters(",")

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()

                For Each field As String In fields
                    Me.RichTextBox1.AppendText(field & " ")
                Next

                Me.RichTextBox1.AppendText(vbLf)
                Me.RichTextBox1.ScrollToCaret()
            End While
        End Using
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.RichTextBox1.Clear()
    End Sub
End Class
