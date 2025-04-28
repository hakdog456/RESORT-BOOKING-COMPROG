Public Class RoomType
    Public Property Name As String
    Public Property Color As String
    Public Property Rooms As New List(Of Room) From {}
    Public Property Price As Double
    Public Property Capacity As Integer


    'CONSTRUCTOR
    Sub New(name As String, color As String)
        Me.Name = name
        Me.Color = color
    End Sub

    Public Function AddRoom(room As Room)
        Me.Rooms.Add(room)
    End Function

End Class
