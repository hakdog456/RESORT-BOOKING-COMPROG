Public Class Account

    Public Property id As String = New Guid().NewGuid.ToString()
    Public Property name As String
    Public Property AccountType As String

    'Constructor
    Sub New(name As String, AccountType As String)
        Me.name = name
        Me.AccountType = AccountType
    End Sub



End Class
