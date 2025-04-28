Imports System.Windows.Markup

Public Class Booking

    Public Property startDate As Date
    Public Property endDate As Date
    Public Property name As String
    Public Property partySize As Integer
    Public Property payment As Double
    Public Property contactNumber As Integer
    Public Property email As String

    Sub New(name As String, contactNumber As Integer, email As String, partySize As Integer, payment As Double, start As Date, endDate As Date)
        Me.name = name
        Me.contactNumber = contactNumber
        Me.email = email
        Me.partySize = partySize
        Me.payment = payment
        Me.startDate = start
        Me.endDate = endDate
    End Sub

    Public Overrides Function ToString() As String
        Return "Check In: " & startDate & " | " & "Check Out: " & endDate
    End Function

End Class
