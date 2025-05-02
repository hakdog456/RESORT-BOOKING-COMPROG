Public Class RoomType
    Public Property Name As String
    Public Property Color As String
    Public Property Rooms As New List(Of Room) From {}
    Public Property Price As Double
    Public Property Capacity As Integer


    'CONSTRUCTOR
    Sub New(name As String, capacity As Integer, price As Double)
        Me.Name = name
        Me.Capacity = capacity
        Me.Price = price
    End Sub

    Public Function AddRoom(name As String)
        Me.Rooms.Add(New Room(name, Me.Name, Me.Capacity, Me.Price, Me))
    End Function

    Public Function removeRoom(room As Room)
        Rooms.Remove(room)
    End Function


End Class
