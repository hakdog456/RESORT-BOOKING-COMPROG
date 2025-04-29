Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Security

Public Class Room
    Implements INotifyPropertyChanged

    Public Property Name As String
    Public Property Type As String
    Public Property Capacity As Integer
    Public Property Price As Double

    Private _active As Boolean = True
    Public Property Active As Boolean
        Get
            Return _active
        End Get
        Set(value As Boolean)
            _active = value
            OnPropertyChanged("getStatusColor")
        End Set
    End Property

    Public Property Bookings As New List(Of Booking) From {}
    Public Property Features As New List(Of String) From {}
    Public Property Pictures As New List(Of Object) From {}
    Public Property Color As String


    'CONSTRUCTOR
    Sub New(
           Name As String,
           Type As String,
           Capacity As Integer,
           Price As Double
           )

        Me.Name = Name
        Me.Type = Type
        Me.Capacity = Capacity
        Me.Price = Price

    End Sub

    'FUNCTIONS
    Public Function statusColor()
        Dim color As String
        If (Active) Then
            color = "#FFA1E6B2"
        Else
            color = "#FFF57879"
        End If

        Return color
    End Function

    'Check if Active
    Public Sub checkStatus(dateToday As Date)
        For Each booking As Booking In Bookings
            If dateToday >= booking.startDate AndAlso dateToday <= booking.endDate Then
                _active = False
                OnPropertyChanged("getStatusColor")
            Else
                If Not _active Then
                    _active = True
                    OnPropertyChanged("getStatusColor")
                    MsgBox("room now avaiable" & dateToday)
                End If
            End If


        Next
    End Sub

    'Property that calls my function
    Public ReadOnly Property getStatusColor As String
        Get
            Return statusColor()
        End Get
    End Property


    'Notify function
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub









End Class
