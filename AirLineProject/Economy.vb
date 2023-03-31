
Imports System.Data.SqlClient
Public Class Economy
    Dim EconomyClass(15, 5) As PictureBox
    Dim EcoSeatinfo(15, 5) As RichTextBox
    Dim EcoEmptySeatImg = "./airlineSeatEmpty.png"
    Dim EcoFullSeatImg = "./airlineSeat.png"
    Dim ConnectionObj As SqlConnection
    Sub PrintLabel(x As Integer)
        For index As Integer = 0 To 5
            If index = 3 Then
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
            x += 160
        Next
    End Sub
    Sub PrintSign(x As Integer, sign As String)
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
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ConnectionObj = New SqlConnection("Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\liliu\source\repos\AirLineProject\AirLineProject\airlineDB.mdf';Integrated Security=True")
        ConnectionObj.Open()

        Dim xLocation As Integer = 125
        Dim yLocation As Integer = 50

        PrintLabel(xLocation)
        PrintSign(555, "Aisle")
        PrintSign(20, "Window")
        PrintSign(1090, "Window")

        xLocation = 50
        For row As Integer = 0 To 15
            For col As Integer = 0 To 5
                EconomyClass(row, col) = New PictureBox() With {
                   .ImageLocation = EcoEmptySeatImg,
                   .Size = New Size(150, 150),
                   .SizeMode = PictureBoxSizeMode.CenterImage,
                   .BackColor = Color.Green,
                   .Location = New Point(xLocation, yLocation),
                   .Name = "PictureBox" + col.ToString + row.ToString,
                   .Visible = True,
                   .Enabled = True
                }
                AddHandler EconomyClass(row, col).Click, AddressOf PictureBoxClick

                EcoSeatinfo(row, col) = New RichTextBox With {
               .Size = New Size(150, 50),
               .Location = New Point(xLocation, yLocation + 150),
               .Name = "RichTextBox" + col.ToString + row.ToString,
               .Visible = True,
               .Enabled = True,
               .Cursor = Cursors.Hand
            }
                Me.Controls.Add(EconomyClass(row, col))
                Me.Controls.Add(EcoSeatinfo(row, col))
                xLocation += 160
                If col = 2 Then
                    xLocation += 100
                Else
                    xLocation += 10
                End If
            Next
            Dim rowLabel As New Label() With {
                .Text = $"{row + 1}",
                .TextAlign = ContentAlignment.MiddleCenter,
                .Location = New Point(15, yLocation + 75),
                .Size = New Size(30, 20),
                .BackColor = Color.Green,
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle
                }
            Me.Controls.Add(rowLabel)
            xLocation = 50
            yLocation += 210
        Next
        '-----------Add Data Reader---------------------
        Dim SelectCmd As New SqlCommand($"SELECT * FROM EconomyClass;", ConnectionObj)
        Dim reader As SqlDataReader
        reader = SelectCmd.ExecuteReader
        If reader.HasRows = False Then
            reader.Close()
            Return
        End If
        While reader.Read
            Dim FirstName As String = Trim(reader.GetValue(1).ToString)
            Dim LastName As String = Trim(reader.GetValue(2).ToString)
            Dim EcoSeat As String = Trim(reader.GetValue(3).ToString)
            Dim row As Integer = Convert.ToInt32(reader.GetValue(4))
            Dim col As Integer = Convert.ToInt32(reader.GetValue(5))
            EconomyClass(row, col).ImageLocation = EcoFullSeatImg
            EcoSeatinfo(row, col).Text = $"Name: {FirstName}_{LastName}{vbCrLf}SeatNumber: {EcoSeat}"
        End While
        reader.Close()
        '------------------------------------------------ 
    End Sub
    Function GetSeatLetter(x) As String
        If x = 0 Then
            Return "A"
        ElseIf x = 1 Then
            Return "B"
        ElseIf x = 2 Then
            Return "C"
        ElseIf x = 3 Then
            Return "D"
        ElseIf x = 4 Then
            Return "E"
        Else
            Return "F"
        End If
    End Function
    Private Sub PictureBoxClick(sender As Object, e As EventArgs)
        Dim PictureBoxClick As PictureBox = sender
        Dim rowNum As Integer = Convert.ToInt32(PictureBoxClick.Name.Substring(11))
        Dim colNum As Integer = Convert.ToInt32(PictureBoxClick.Name.Substring(10, 1))
        If TextBox1.Text <> "" And TextBox2.Text <> "" And EcoSeatinfo(rowNum, colNum).Text = "" Then
            Dim YesNoSubmit As DialogResult = MessageBox.Show("Do You Want To Submit Your Reservation?", "Submit Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If YesNoSubmit = DialogResult.Yes Then
                PictureBoxClick.ImageLocation = EcoFullSeatImg
                EcoSeatinfo(rowNum, colNum).Text = $"Name: {TextBox1.Text}_{TextBox2.Text}{vbCrLf}Seat Number: {rowNum + 1}{GetSeatLetter(colNum)}"
                '----------------Insert Data into Database--------------------------
                Dim SeatID As String = colNum.ToString + rowNum.ToString
                Dim InsString As String = $"INSERT INTO EconomyClass (SeatID,FirstName,LastName,EcoSeat,Row,Col) VALUES ('{SeatID}', '{TextBox1.Text}','{TextBox2.Text}',' {rowNum + 1}{GetSeatLetter(colNum)}',{rowNum},{colNum});"
                Dim InsCmd As New SqlCommand(InsString, ConnectionObj)
                InsCmd.ExecuteNonQuery()
                MessageBox.Show("Record Added!!")
                '----------------------------------------------------

                TextBox1.Clear()
                TextBox2.Clear()
            Else
                MessageBox.Show("Input Passenger's Name Then Select A Seat")
            End If
        ElseIf EcoSeatinfo(rowNum, colNum).Text <> "" Then
            Dim YesNoCancel As DialogResult = MessageBox.Show("Are You Sure To Cancel Your Selection?", "Cancel Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If YesNoCancel = DialogResult.Yes Then
                '-------------Delete Data From Database-------
                Dim delString As String = $"DELETE FROM EconomyClass WHERE Row='{rowNum}' and Col='{colNum}';"
                Dim delCmd As New SqlCommand(delString, ConnectionObj)
                delCmd.ExecuteNonQuery()
                MessageBox.Show("Record Delete Successfully!!")
                '---------------------------------------------

                PictureBoxClick.ImageLocation = EcoEmptySeatImg
                EcoSeatinfo(rowNum, colNum).Text = ""
            Else
                Return
            End If
        ElseIf TextBox1.text = "" And TextBox2.text <> "" Then
            MessageBox.Show("Input Passenger's First Name Then Select A Seat")
        ElseIf TextBox2.text = "" And TextBox1.text <> "" Then
            MessageBox.Show("Input Passenger's Last Name Then Select A Seat")
        Else
            MessageBox.Show("Input Passenger's Name Then Select A Seat")
        End If
    End Sub
End Class