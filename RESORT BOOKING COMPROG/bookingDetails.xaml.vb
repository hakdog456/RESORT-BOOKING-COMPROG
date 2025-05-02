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

    Public Sub New(mainWindow As MainWindow)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mainWindow = TryCast(Application.Current.MainWindow, MainWindow)
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        mainGrid.DataContext = Me

    End Sub

    Private Sub forceCheckOutBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles forceCheckOutBtn.MouseDown
        RaiseEvent removeBookingFromMain(Me, EventArgs.Empty)
    End Sub
End Class
