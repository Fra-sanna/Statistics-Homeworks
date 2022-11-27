Public Class Form1

    Public b, b2 As Bitmap
    Public g, g2 As Graphics
    Public r As New Random


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Me.b = New Bitmap(Me.PictureBox1.Width, Me.PictureBox1.Height)
        Me.g = Graphics.FromImage(b)
        Me.g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

        Me.b2 = New Bitmap(Me.PictureBox2.Width, Me.PictureBox2.Height)
        Me.g2 = Graphics.FromImage(b2)
        Me.g2.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

        Me.g.Clear(Color.White)
        Me.g2.Clear(Color.White)

        Dim TrialsCount As Integer = Me.NumericUpDown1.Value
        Dim NumerOfTrajectories As Integer = Me.NumericUpDown2.Value
        Dim SuccessProbability As Double = 0.5

        Dim minX As Double = 0
        Dim maxX As Double = TrialsCount
        Dim minY As Double = 0
        Dim maxY As Double = TrialsCount
        Dim absolutefreq As Integer = 0

        Dim VirtualWindow As New Rectangle(0, 0, Me.b.Width, Me.b.Height)

        g.DrawRectangle(Pens.Black, VirtualWindow)
        g2.DrawRectangle(Pens.Black, VirtualWindow)

        Dim average As Integer
        Dim YDevice2 As Integer
        Dim xDevice As Integer
        Dim dictaverage As New Dictionary(Of Integer, Integer)

        For i As Integer = 0 To NumerOfTrajectories

            Dim success As Double = 0

            For X As Integer = 1 To TrialsCount
                Dim Uniform As Double = r.NextDouble
                If Uniform < SuccessProbability Then
                    success += 1
                Else

                End If
                xDevice = FromXRealToXVirtual(X, minX, maxX, VirtualWindow.Left, VirtualWindow.Width)

                average = success * TrialsCount / (X + 1)
                YDevice2 = FromYRealToYVirtual(average, minY, maxY, VirtualWindow.Top, VirtualWindow.Height)

            Next

            If dictaverage.ContainsKey(YDevice2) Then
                dictaverage(YDevice2) += 10
            Else
                dictaverage.Add(YDevice2, 10)
            End If

        Next
        g2.TranslateTransform(0, b2.Height)
        g2.ScaleTransform(1, -1)
        For Each item In dictaverage
            Dim i As Integer = 0
            Dim rect As New Rectangle(item.Key, 0, 1, item.Value)
            Dim the_brush As New SolidBrush(Color.Green)
            Dim the_pen As New Pen(Color.Green, 0)
            g2.FillRectangle(the_brush, rect)
            g2.DrawRectangle(the_pen, rect)

        Next

        For Each item In dictaverage
            Dim rect As New Rectangle(0, item.Key, item.Value, 1)
            Dim the_brush As New SolidBrush(Color.Green)
            Dim the_pen As New Pen(Color.Green, 0)
            g.FillRectangle(the_brush, rect)
            g.DrawRectangle(the_pen, rect)

        Next

        Me.PictureBox1.Image = b
        Me.PictureBox2.Image = b2

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged

    End Sub

    Function FromXRealToXVirtual(X As Double,
                                 minX As Double, maxX As Double,
                                 Left As Integer, W As Integer) As Integer

        If (maxX - minX) = 0 Then
            Return 0
        End If

        Return Left + W * (X - minX) / (maxX - minX)

    End Function

    Function FromYRealToYVirtual(Y As Double,
                                minY As Double, maxY As Double,
                                Top As Integer, H As Integer) As Integer

        If (maxY - minY) = 0 Then
            Return 0
        End If

        Return Top + H - H * (Y - minY) / (maxY - minY)

    End Function

End Class
