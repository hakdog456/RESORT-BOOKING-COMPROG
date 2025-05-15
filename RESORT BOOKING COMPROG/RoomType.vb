Imports System.Collections.ObjectModel
Imports System.ComponentModel


Public Class RoomType
    Implements INotifyPropertyChanged

    Public Property Name As String
    Public Property id As String = New Guid().NewGuid.ToString()
    Public Property Color As String
    Public Property Rooms As New ObservableCollection(Of Room) From {}
    Public Property Price As Double
    Public Property Capacity As Integer
    Public Property features As New List(Of String) From {}

    Private _RoomsCount As Integer = Rooms.Count
    Public Property RoomsCount As Integer
        Get
            Return _RoomsCount
        End Get
        Set(value As Integer)
            _RoomsCount = value
            OnPropertyChanged("RoomsCount")
        End Set
    End Property

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
    Sub New(name As String, capacity As Integer, price As Double)
        Me.Name = name
        Me.Capacity = capacity
        Me.Price = price
    End Sub

    Public Function AddRoom(name As String)
        Dim room As Room = New Room(name, Me.Name, Me.Capacity, Me.Price, Me)
        room.roomTypeId = id
        Me.Rooms.Add(room)
        Me.RoomsCount = Rooms.Count
    End Function

    Public Function AddRoom(room As Room)
        room.roomTypeId = id
        room.Promos = Me.Promos
        Me.Rooms.Add(room)
        Me.RoomsCount = Rooms.Count
    End Function

    Public Function removeRoom(room As Room)
        Me.Rooms.Remove(room)
        Me.RoomsCount = Rooms.Count
    End Function

    Public Overrides Function ToString() As String
        Return Name
    End Function


    'Notify function
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

End Class
