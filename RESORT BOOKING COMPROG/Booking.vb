Public Class Booking

    Public Property startDate As Date
    Public Property endDate As Date
    Public Property name As String
    Public Property partySize As Integer

    Sub New(start As Date, endDate As Date)
        Me.startDate = start
        Me.endDate = endDate
    End Sub

    Public Overrides Function ToString() As String
        Return "Check In: " & startDate & " | " & "Check Out: " & endDate
    End Function

End Class
