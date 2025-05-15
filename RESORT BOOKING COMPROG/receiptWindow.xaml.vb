

Public Class receiptWindow

    Public Property CustomerName As String
    Public Property CustomerEmail As String
    Public Property CustomerContact As String
    Public Property RoomName As String
    Public Property RoomType As String
    Public Property Start As String
    Public Property Ends As String
    Public Property DayCount As String
    Public Property RoomOccupancy As String
    Public Property PartySize As Integer
    Public Property RoomCost As Integer
    Public Property Nights As Integer
    Public Property Subtotal As Integer
    Public Property Payment As Integer
    Public Property displayStat As String
    Public Property bookingId As String

    Private _Promos As New List(Of Promo) From {}

    Public Property Promos As List(Of Promo)
        Get
            Return _Promos
        End Get
        Set(value As List(Of Promo))
            _Promos = value
        End Set
    End Property





    Private Sub receiptWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim details As String = CustomerName & Environment.NewLine &
                        CustomerEmail & Environment.NewLine &
                         CustomerContact
        CustomerDetails.Text = details

        Dim Bookings As String = RoomName & Environment.NewLine & RoomType & Environment.NewLine &
            Start & Environment.NewLine & Ends & Environment.NewLine & DayCount & Environment.NewLine & RoomOccupancy & Environment.NewLine & PartySize
        BookingDetailss.Text = Bookings

        Dim Summary As String = CustomerName & Environment.NewLine & RoomCost & Environment.NewLine & Nights & Environment.NewLine & Subtotal & Environment.NewLine
        SummarContent.Text = Summary

        'Random number generator for payment id
        Dim rnd As New Random()
        Dim iDnum As Integer = rnd.Next(10000, 100000)

        'calculate balance
        Dim ToPay = Subtotal - Payment


        'Assigning values to payment details display
        IDnumber.Content = iDnum.ToString()
        StartDate.Content = Start
        displayStats.Content = displayStat
        Remaining.Content = "₱" & ToPay.ToString

        If ToPay < 0 Then
            AmountPaid.Content = "₱" & Subtotal
            TotalPaid.Content = "₱" & Subtotal
        Else
            AmountPaid.Content = "₱" & Payment
            TotalPaid.Content = "₱" & Payment
        End If




    End Sub


End Class
