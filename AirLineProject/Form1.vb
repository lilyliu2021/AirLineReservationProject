Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim BusinessSeat As Business = New Business
        BusinessSeat.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim EconomySeat As Economy = New Economy
        EconomySeat.Show()
    End Sub
End Class