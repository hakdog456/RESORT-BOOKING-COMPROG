Public Class Discount

    Private _Name
    Public Property Name As String
        Get
            Return _Name & "         "
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property
    Public Property Amount As Double

    Sub New(name As String, amount As Double)
        Me.name = name
        Me.amount = amount

    End Sub

End Class
