Public Class Account

    Public Property id As String = New Guid().NewGuid.ToString()
    Public Property name As String
    Public Property AccountType As String

    Public Property usn As String
    Public Property pass As String


    'Constructor
    Sub New(name As String, AccountType As String, usn As String, pass As String)
        Me.name = name
        Me.AccountType = AccountType
        Me.usn = usn
        Me.pass = pass
    End Sub



End Class
