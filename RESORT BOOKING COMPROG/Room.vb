Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Security
Imports System.Collections.ObjectModel

Public Class Room
    Implements INotifyPropertyChanged

    Public Property Name As String
    Public Property Type As String
    Public Property roomType As RoomType
    Public Property id As String = New Guid().NewGuid.ToString()
    Public Property roomTypeId As String

    Private _capacity As Integer
    Public Property Capacity As Integer
        Get
            Return _capacity
        End Get
        Set(value As Integer)
            _capacity = value
        End Set
    End Property
    Public Property Price As Double

    Private _active As Boolean = True
    Public Property Active As Boolean
        Get
            Return _active
        End Get
        Set(value As Boolean)
            _active = value
            OnPropertyChanged("getStatusColor")
            OnPropertyChanged("statusText")
        End Set
    End Property

    'Public Property Bookings As New List(Of Booking) From {}
    Public Property Bookings As New ObservableCollection(Of Booking) From {}
    Public Property Features As New List(Of String) From {}

    Public Property FeaturesString As String
        Get
            Dim featuresToString As String = String.Join(", ", Features)
            Return featuresToString
        End Get
        Set(value As String)
            FeaturesString = value
        End Set
    End Property

    Public Property Pictures As New List(Of RoomPicture) From {}
    Public Property Color As String

    Private _statusText As String = "Available"
    Public Property statusText As String
        Get
            Return _statusText
        End Get
        Set(value As String)
            _statusText = value
            OnPropertyChanged("statusText")
            OnPropertyChanged("getStatusColor")
        End Set
    End Property

    'Property that calls my function color
    Public ReadOnly Property getStatusColor As String
        Get
            Return statusColor()
        End Get
    End Property

    'Promos
    Private _Promos As New List(Of Promo) From {}

    Public Property Promos As List(Of Promo)
        Get
            Return _Promos
        End Get
        Set(value As List(Of Promo))
            _Promos = value
            OnPropertyChanged("Promo")
        End Set
    End Property


    'CONSTRUCTOR
    Sub New(
           Name As String,
           Type As String,
           Capacity As Integer,
           Price As Double,
           roomType As RoomType
           )

        Me.Name = Name
        Me.Type = Type
        Me.roomType = roomType
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

    'Add Booking 
    Public Sub addBooking(booking As Booking)
        booking.roomId = id
        Bookings.Add(booking)
    End Sub

    'Check if Active
    Public Sub checkStatus(dateToday As Date)
        Dim bookedToday As Boolean = False

        For Each booking As Booking In Bookings
            If dateToday >= booking.startDate AndAlso dateToday <= booking.endDate Then
                bookedToday = True
                Exit For

            End If
        Next

        If bookedToday Then
            _active = False
            _statusText = "Unvailable"
            OnPropertyChanged("getStatusColor")
            OnPropertyChanged("statusText")

        Else
            If _active = False Then
                _active = True
                _statusText = "Available"
                OnPropertyChanged("getStatusColor")
                OnPropertyChanged("statusText")
                MsgBox("room now avaiable" & dateToday)

            End If

        End If


    End Sub

    'remove Past Booking 
    Public Sub removePastBookings(dateToday As Date)
        Dim bookingsDup As New List(Of Booking)(Bookings)

        For Each booking As Booking In bookingsDup
            If booking.endDate < dateToday Then
                'code for putting past bookings in db

                Bookings.Remove(booking)
            End If
        Next
    End Sub

    'remove a booking
    Public Sub removeBooking(bookingToRemove As Booking)
        Bookings.Remove(bookingToRemove)
    End Sub


    'Notify function
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub









End Class
