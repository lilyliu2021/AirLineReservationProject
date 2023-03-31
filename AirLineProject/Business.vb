Imports System.Data.SqlClient  'Add using to access the SQl Client and data libraries
Public Class Business
    Dim BusinessClass(20) As PictureBox
    ' Dim myImageLocationPrefix As String = "C:\Users\liliu\source\repos\AirLineProject\AirLineProject\bin\Debug\net6.0-windows\"
    Dim EmptySeatImg As String = "./airlineSeatEmpty.png"
    Dim FullSeatImg As String = "./airlineSeat.png"
    Dim BusiSeatinfo(20) As RichTextBox
    Dim ConnectionObj As SqlConnection

    Sub PrintLabel(x As Integer)
        For index As Integer = 0 To 3
            If index = 2 Then
                x += 100
            End If
            Dim colLabel As New Label() With {
             .Text = $"{GetSeatLetter(index)}",
                .TextAlign = ContentAlignment.MiddleCenter,
                .Location = New Point(x, 20),
                .Size = New Size(25, 25),
                .BackColor = Color.Green,
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle
            }
            Me.Controls.Add(colLabel)
            x += 210
        Next
    End Sub
    Sub PrintSign(x As Integer, sign As String)
        'Dim aisleLabel As Label
        'aisleLabel = New Label() With {
        Dim aisleLabel As New Label() With {
        .Text = sign,
                    .TextAlign = ContentAlignment.MiddleCenter,
                    .Location = New Point(x, 20),
                    .Size = New Size(80, 25),
                    .BackColor = Color.Blue,
                    .ForeColor = Color.White,
                    .BorderStyle = BorderStyle.FixedSingle
                }
        Me.Controls.Add(aisleLabel)
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ConnectionObj = New SqlConnection("Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\liliu\source\repos\AirLineProject\AirLineProject\airlineDB.mdf';Integrated Security=True")
        ConnectionObj.Open()

        Dim xLocation As Integer = 150
        Dim yLocation As Integer = 50
        Dim line As Integer = 1

        PrintLabel(xLocation)
        PrintSign(480, "Aisle")
        PrintSign(20, "Window")
        PrintSign(950, "Window")
        xLocation = 50
        For index = 0 To 19
            BusinessClass(index) = New PictureBox() With {
            .ImageLocation = EmptySeatImg,
            .Size = New Size(200, 200),
            .BackColor = Color.Yellow,
            .SizeMode = PictureBoxSizeMode.CenterImage,
            .Location = New Point(xLocation, yLocation),
            .Name = "PictureBox" + $"{line}" + index.ToString(),
            .Visible = True,
            .Enabled = True,
            .Cursor = Cursors.Hand
        }
            AddHandler BusinessClass(index).Click, AddressOf PictureBoxClick
            BusiSeatinfo(index) = New RichTextBox With {
               .Size = New Size(200, 80),
               .Location = New Point(xLocation, yLocation + 200),
               .Name = "RichTextBox" + index.ToString(),
               .Visible = True,
               .Enabled = True,
               .Cursor = Cursors.Hand
            }

            Me.Controls.Add(BusinessClass(index))
            Me.Controls.Add(BusiSeatinfo(index))

            xLocation += 210
            'If (index + 1) Mod 4 = 0 Then
            If (index + 1) Mod 2 = 0 And (index + 1) Mod 4 <> 0 Then
                xLocation += 120
                ' yLocation += 210
            ElseIf (index + 1) Mod 4 = 0 Then
                Dim rowLabel As New Label() With {
                .Text = $"{line}",
                .TextAlign = ContentAlignment.MiddleCenter,
                .Location = New Point(15, yLocation + 100),
                .Size = New Size(30, 20),
                .BackColor = Color.Green,
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle
                }
                Me.Controls.Add(rowLabel)
                line += 1
                xLocation = 50
                yLocation += 290
            End If
        Next

        '------------Add database related code
        Dim SelectCmd As New SqlCommand($"SELECT * FROM BusinessClass;", ConnectionObj)
        Dim reader As SqlDataReader
        reader = SelectCmd.ExecuteReader
        If reader.HasRows = False Then
            reader.Close()
            Return
        End If
        While reader.Read
            Dim seat As Integer = Convert.ToInt32(reader.GetValue(0))
            Dim firstName As String = Trim(reader.GetValue(1).ToString)
            Dim lastName As String = Trim(reader.GetValue(2).ToString)
            Dim seatNum As String = Trim(reader.GetValue(3).ToString)
            BusinessClass(seat).ImageLocation = FullSeatImg
            BusiSeatinfo(seat).Text = $"Name: {firstName}_{lastName}{vbCrLf}SeatNumber: {seatNum}"
        End While
        reader.Close()
        '-------------------------------------
    End Sub
    Function GetSeatLetter(x) As String
        Dim numX = Convert.ToInt32(x)
        If numX Mod 4 = 0 Then
            Return "A"
        ElseIf numX Mod 4 = 1 Then
            Return "B"
        ElseIf numX Mod 4 = 2 Then
            Return "C"
        Else
            Return "D"
        End If
    End Function
    Private Sub PictureBoxClick(sender As Object, e As EventArgs)
        Dim PictureBoxClick As PictureBox = sender
        Dim seatNum As String = PictureBoxClick.Name.Substring(11)
        Dim lineNum As String = PictureBoxClick.Name.Substring(10, 1)
        If TextBox1.Text <> "" And TextBox2.Text <> "" And BusiSeatinfo(seatNum).Text = "" Then
            Dim YesNoSubmit As DialogResult = MessageBox.Show("Do You Want To Submit Your Reservation?", "Submit Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If YesNoSubmit = DialogResult.Yes Then
                PictureBoxClick.ImageLocation = FullSeatImg
                BusiSeatinfo(seatNum).Text = $"Name: {TextBox1.Text}_{TextBox2.Text}{vbCrLf}Seat Number: {lineNum}{GetSeatLetter(seatNum)}"
                '------------Add database related code
                Dim insertSqlString As String = $"INSERT INTO BusinessClass (SeatID,FirstName,LastName,BizSeat) VALUES ({seatNum},'{TextBox1.Text}','{TextBox2.Text}','{lineNum}{GetSeatLetter(seatNum)}');"

                Dim InsertCmd As New SqlCommand(insertSqlString, ConnectionObj)
                InsertCmd.ExecuteNonQuery()
                MessageBox.Show("Record Added")

                '-------------------------------------
                TextBox1.Clear()
                TextBox2.Clear()
            Else
                MessageBox.Show("Input Passenger's Name Then Select A Seat")
            End If
        ElseIf BusiSeatinfo(seatNum).Text <> "" Then
            Dim YesNoCancel As DialogResult = MessageBox.Show("Are You Sure To Cancel Your Selection?", "Cancel Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If YesNoCancel = DialogResult.Yes Then
                PictureBoxClick.ImageLocation = EmptySeatImg
                BusiSeatinfo(seatNum).Text = ""
                '---------------Add database related code
                Dim delSqlString As String = $"DELETE FROM BusinessClass WHERE SeatID='{seatNum}';"
                Dim DelCmd As New SqlCommand(delSqlString, ConnectionObj)
                DelCmd.ExecuteNonQuery()
                MessageBox.Show("Record Delete Successfully!!")
                '-----------------------------------
            Else
                Return
            End If
        ElseIf TextBox1.Text = "" And TextBox2.Text <> "" Then
            MessageBox.Show("Input Passenger's First Name Then Select A Seat")
        ElseIf TextBox2.Text = "" And TextBox1.Text <> "" Then
            MessageBox.Show("Input Passenger's Last Name Then Select A Seat")
        Else
            MessageBox.Show("Input Passenger's Name Then Select A Seat")
        End If
    End Sub
End Class
