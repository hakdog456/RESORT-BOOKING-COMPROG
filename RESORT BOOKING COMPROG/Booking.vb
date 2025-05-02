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

    Sub New(room As Room, roomName As String, roomType As String, name As String, contactNumber As Integer, email As String, partySize As Integer, payment As Double, start As Date, endDate As Date)
        Me.room = room
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

End Class
