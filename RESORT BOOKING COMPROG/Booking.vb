Imports System.Windows.Markup
Imports System.ComponentModel


Public Class Booking
    Implements INotifyPropertyChanged

    Public Property startDate As Date
    Public Property endDate As Date
    Public Property name As String
    Public Property partySize As Integer
    Public Property payment As Double
    Public Property contactNumber As String
    Public Property email As String
    Public Property roomName As String
    Public Property roomType As String
    Public Property room As Room
    Public Property id As String
    Public Property roomId As String
    Public Property roomTypeId As String

    Public Property humanType As String = "Adult"
    Public Property days As Integer
    Public Property receipt As receiptWindow
    Public Property color As String

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



    Sub New(room As Room, days As Integer, roomName As String, roomType As String, name As String, contactNumber As String, email As String, partySize As Integer, payment As Double, start As Date, endDate As Date)
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

    'show Receipt Function
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
        showWind.Promos = Promos

        If CInt(payment) >= CInt(showWind.Subtotal) Then
            showWind.displayStat = "Paid"
        ElseIf CInt(payment) <= CInt(showWind.Subtotal) And payment > 0 Then
            showWind.displayStat = "Partially Paid"
        End If

        showWind.Show()
    End Sub

    'Add Promo
    Public Sub addPromo(name As String, type As String, value As Double, amount As Integer)
        Dim Promo As New Promo(name, type, value, amount)
        Promo.bookingId = id
        Promos.Add(Promo)
    End Sub

    'notify property Change
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub




End Class
