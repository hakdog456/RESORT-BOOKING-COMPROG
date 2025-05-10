Imports System.Windows.Markup

Public Class Booking

    Public Property startDate As Date
    Public Property endDate As Date
    Public Property name As String
    Public Property partySize As Integer
    Public Property payment As Double
    Public Property contactNumber As Integer
    Public Property email As String
    Public Property roomName As String
    Public Property roomType As String
    Public Property room As Room
    Public Property id As String
    Public Property humanType As String = "Adult"
    Public Property days As Integer
    Public Property receipt As receiptWindow



    Sub New(room As Room, days As Integer, roomName As String, roomType As String, name As String, contactNumber As Integer, email As String, partySize As Integer, payment As Double, start As Date, endDate As Date)
        Me.room = room
        Me.days = days
        Me.roomName = roomName
        Me.roomType = roomType
        Me.name = name
        Me.contactNumber = contactNumber
        Me.email = email
        Me.partySize = partySize
        Me.payment = payment
        Me.startDate = start
        Me.endDate = endDate
        Me.id = Guid.NewGuid().ToString()
    End Sub

    Public Overrides Function ToString() As String
        Return "Check In: " & startDate & " | " & "Check Out: " & endDate
    End Function

    Public Sub showReceipt()
        Dim showWind As New receiptWindow()
        showWind.CustomerName = name
        showWind.CustomerEmail = email
        showWind.CustomerContact = contactNumber
        showWind.RoomName = room.Name
        showWind.RoomType = room.Type
        showWind.Start = startDate
        showWind.Ends = endDate
        showWind.DayCount = days
        showWind.RoomOccupancy = room.Capacity
        showWind.PartySize = partySize
        showWind.RoomCost = room.Price
        showWind.Nights = days
        showWind.Subtotal = room.Price * days
        showWind.Payment = payment

        If CInt(payment) >= CInt(showWind.Subtotal) Then
            showWind.displayStat = "Paid"
        ElseIf CInt(payment) <= CInt(showWind.Subtotal) And payment > 0 Then
            showWind.displayStat = "Partially Paid"
        End If

        showWind.Show()
    End Sub




End Class
