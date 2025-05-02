Public Class bookingDetails

    Public Property customerName As String = "--"
    Public Property number As String = "--"
    Public Property email As String = "--"
    Public Property humanType As String = "--"
    Public Property roomType As String = "--"
    Public Property roomName As String = "--"
    Public Property roomCapacity As String = "--"
    Public Property days As String = "--"
    Public Property partySize As String = "--"
    Public Property id As String = "--"
    Public Property startDate As Date
    Public Property endDate As Date
    Public Property booking As Booking
    'Public Property promos As Promo
    Public Event removeBookingFromMain As EventHandler

    Public Sub New(mainWindow As MainWindow, bookingFromMain As Booking)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mainWindow = TryCast(Application.Current.MainWindow, MainWindow)
        booking = bookingFromMain
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        If booking.name IsNot "" Then
            customerName = booking.name
        Else
            customerName = "--"
        End If

        If booking.contactNumber Then
            number = booking.contactNumber
        Else
            number = "--"
        End If

        If booking.email IsNot "" Then
            email = booking.email
        Else
            email = "--"
        End If

        If booking.humanType IsNot "" Then
            humanType = booking.humanType
        Else
            humanType = "--"
        End If

        If booking.roomType IsNot "" Then
            roomType = booking.roomType
        Else
            roomType = "--"
        End If

        If booking.roomName IsNot "" Then
            roomName = booking.roomName
        Else
            roomName = "--"
        End If

        If booking.room.Capacity Then
            roomCapacity = booking.room.Capacity
        Else
            roomCapacity = "--"
        End If

        If booking.days Then
            days = booking.days
        Else
            days = "--"
        End If

        If booking.partySize Then
            partySize = booking.partySize
        Else
            partySize = "--"
        End If

        id = booking.id
        'startDate
        'endDate

        'transfering the datacontext to window
        mainGrid.DataContext = Me

    End Sub

    Private Sub forceCheckOutBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles forceCheckOutBtn.MouseDown
        RaiseEvent removeBookingFromMain(Me, EventArgs.Empty)
        Me.Hide()
    End Sub
End Class
