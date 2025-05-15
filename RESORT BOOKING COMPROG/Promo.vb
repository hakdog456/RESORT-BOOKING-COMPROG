Imports System.ComponentModel


Public Class Promo
    Implements INotifyPropertyChanged

    Public Property id As String = New Guid().NewGuid().ToString()
    Public Property promoClassId = New Guid().NewGuid().ToString()
    Public Property bookingId As String

    Private _name As String
    Public Property name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            OnPropertyChanged("name")
        End Set
    End Property

    Private _type As String
    Public Property type As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
            OnPropertyChanged("type")
        End Set
    End Property

    Private _value As Double
    Public Property value As Double
        Get
            Return _value
        End Get
        Set(value As Double)
            _value = value
            OnPropertyChanged("value")
        End Set
    End Property

    Private _amount As Integer
    Public Property amount As Integer
        Get
            Return _amount
        End Get
        Set(value As Integer)
            _amount = value
            OnPropertyChanged("amount")
            OnPropertyChanged("amountText")
        End Set
    End Property

    Public Property amountText As String
        Get
            Dim result As String = ""
            If amount >= 1000 Then
                result = "Unli"
            Else
                result = amount
            End If
            Return result
        End Get
        Set(value As String)

        End Set
    End Property

    Public Property discountText As String
        Get
            Dim mode As String = ""
            If type = "percentage" Then
                mode = "%"
            End If
            Return "-" & value & mode
        End Get
        Set(value As String)

        End Set
    End Property


    'Container
    Sub New(name As String, type As String, value As Double, amount As Integer)
        Me.name = name
        Me.type = type
        Me.value = value
        Me.amount = amount
    End Sub

    Sub New(promo As Promo)
        Me.name = promo.name
        Me.type = promo.type
        Me.value = promo.value
        Me.amount = promo.amount
    End Sub


    'Notify Function
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

End Class
