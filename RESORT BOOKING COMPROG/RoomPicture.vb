Public Class RoomPicture

    Public Property roomId As String
    Public Property roomTypeId As String

    Public Property imageSource As ImageSource

    'CONSTRUCTOR
    Sub New(roomId As String, roomTypeId As String, imageSource As ImageSource)
        Me.roomId = roomId
        Me.roomTypeId = roomTypeId
        Me.imageSource = imageSource
    End Sub

End Class
