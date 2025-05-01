Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports MS.Internal.Text.TextInterface


Class MainWindow
    Implements INotifyPropertyChanged


    'VARIABLES
    'PAGES
    Public Property pages As New List(Of String) From {"login", "main"}

    'MAIN PAGE VIEWS
    Dim views As New List(Of Grid) From {POS, Calendar, Dashboard, Room, Security}

    'NAV BUTTONS
    Dim navBtns As New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}

    'POS WINDOWS
    Dim posWindows As New List(Of Grid) From {PosRoomCheck, PosRoomDetailsGrid, PosRoomBookingGrid}

    'BOOKINGS
    Dim bookings As New List(Of Booking) From {}

    'ROOMS
    Dim rooms As List(Of Room)

    'ROOM TYPES
    Dim roomTypes As New ObservableCollection(Of RoomType)


    'Timers
    Private bookingTimer As DispatcherTimer

    'VARIABLE SELECTIONS
    'Public Property selectedRoom As Room
    Private selectedRoom As Room

    'INITIALIZATIONS
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Me
        selectedRoom = New Room("ojnanw", "ojbae", 1, 0)

        'Timerz
        bookingTimer = New DispatcherTimer()
        bookingTimer.Interval = TimeSpan.FromSeconds(1)
        AddHandler bookingTimer.Tick, AddressOf bookingTimer_Tick
        bookingTimer.Start()

    End Sub

    'PRORTY CHANGE
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    'FUNCTIONS
    'TIMERZ
    Sub bookingTimer_Tick()

        Dim available As Integer = 0
        Dim unavailable As Integer = 0

        Dim dateToday = New Date().Now
        getRooms()

        'ROOMS ITERATION
        For Each room As Room In rooms
            'Check if a Room is booked today
            room.checkStatus(dateToday)

            'Update counts of available and unavailabls 
            If room.Active Then
                available += 1
            Else
                unavailable += 1
            End If

            'Delete Past Bookings and Store in DB - past Bookings
            room.removePastBookings(dateToday)

        Next

        availableRooms.Content = available
        unavailableRooms.Content = unavailable

    End Sub

    'GET ALL BOOKINGS
    Sub getRooms()

        Dim roomsDup As New List(Of Room) From {}
        For Each roomType As RoomType In roomTypes
            roomsDup.AddRange(roomType.Rooms)
        Next
        rooms = roomsDup
    End Sub

    'GET ALL BOOKINGS
    Sub getBookings()
        getRooms()

        Dim bookingsDup As New List(Of Booking) From {}
        For Each room As Room In rooms
            bookingsDup.AddRange(room.Bookings)
        Next
        bookings = bookingsDup

    End Sub

    'SELECTING VIEWS FUNCTION
    Sub selectView(selected As Grid)
        For Each grid As Grid In views
            If selected Is grid Then
                grid.Visibility = Visibility.Visible
            Else
                grid.Visibility = Visibility.Collapsed
            End If

        Next

    End Sub

    'GENERIC VIEW SELECTOR FOR GRIDS
    Sub selectViewGeneric(selected As Object, list As List(Of Grid))
        For Each grid As Grid In list
            If selected Is grid Then
                grid.Visibility = Visibility.Visible
            Else
                grid.Visibility = Visibility.Collapsed
            End If
        Next

    End Sub

    'SETTING BUTTON BGS
    Sub setBtnBg(btn As Border)
        For Each border As Border In navBtns
            If border Is btn Then
                border.Background = New SolidColorBrush((ColorConverter.ConvertFromString("#07380A")))

            Else
                border.Background = Nothing
            End If
        Next
    End Sub

    'WHEN WINDOW IS LOADED 
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Hiding main page
        mainPageGrid.Visibility = Visibility.Collapsed
        PosRoomBookingGrid.Visibility = Visibility.Collapsed
        PosRoomDetailsGrid.Visibility = Visibility.Collapsed

        'Reinitializing views of grid
        views = New List(Of Grid) From {POS, Calendar, Dashboard, Room, Security}
        navBtns = New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}
        posWindows = New List(Of Grid) From {PosRoomCheck, PosRoomDetailsGrid, PosRoomBookingGrid}


        'Adding room types and rooms per room type
        Dim regular As New RoomType("REGULAR", 3, 1200)
        regular.AddRoom("R101")
        regular.AddRoom("R102")
        regular.AddRoom("R103")
        regular.AddRoom("R104")
        regular.AddRoom("R105")
        regular.AddRoom("R106")
        regular.AddRoom("R107")

        Dim premium As New RoomType("PREMIUM", 5, 3200)
        premium.AddRoom("P101")
        premium.AddRoom("P102")
        premium.AddRoom("P103")
        premium.AddRoom("P104")
        premium.AddRoom("P105")

        Dim deluxe As New RoomType("DELUXE", 10, 10000)
        deluxe.AddRoom("D101")
        deluxe.AddRoom("D102")
        deluxe.AddRoom("D103")
        deluxe.AddRoom("D104")
        deluxe.AddRoom("D105")
        deluxe.AddRoom("D106")

        Dim bembang As New RoomType("BEMBANG", 2, 2000)
        bembang.AddRoom("B101")
        bembang.AddRoom("B102")
        bembang.AddRoom("B103")
        bembang.AddRoom("B104")
        bembang.AddRoom("B105")
        bembang.AddRoom("B106")
        bembang.AddRoom("B107")
        bembang.AddRoom("B108")

        roomTypes.Add(regular)
        roomTypes.Add(premium)
        roomTypes.Add(deluxe)
        roomTypes.Add(bembang)

        'Assigning Roomtypes as item source for RoomTypeListBox
        RoomTypeListBox.ItemsSource = roomTypes


        'Assigning Starting Dates for POS date picker
        startDate.SelectedDate = Date.Now

        'Setting the item source for calendar Bookings
        'calndarBookings.ItemsSource = 



    End Sub


    'Get Bookings based on Date
    Public Function getBookingsOnDate(givenDate As Date)
        Dim bookingsOnDate As New List(Of Booking)

        getRooms()
        getBookings()

        For Each booking As Booking In bookings
            If givenDate >= booking.startDate AndAlso givenDate <= booking.endDate Then
                bookingsOnDate.Add(booking)

            End If
        Next

        Return bookingsOnDate

    End Function


    'LOG IN BUTTON
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'MsgBox("Logging in")
        loginPage.Visibility = Visibility.Collapsed
        mainPageGrid.Visibility = Visibility.Visible
    End Sub

    'POS BUTTON CLICKED
    Private Sub posSide_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles posSide.MouseDown
        selectView(POS)
        setBtnBg(posSide)
    End Sub

    'CALENDAR BUTTON CLICKED

    Private Sub calSide_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles calSide.MouseDown
        selectView(Calendar)
        setBtnBg(calSide)

    End Sub

    'ROOM MANAGEMENT BUTTON CLICKED
    Private Sub roomSide_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles roomSide.MouseDown
        selectView(Room)
        setBtnBg(roomSide)

    End Sub

    'SECURITY BUTTON CLICKED
    Private Sub secSide_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles secSide.MouseDown
        selectView(Security)
        setBtnBg(secSide)

    End Sub

    'DASHBOARD BUTTON CLICKED
    Private Sub dashSide_MouseDown_1(sender As Object, e As MouseButtonEventArgs) Handles dashSide.MouseDown
        selectView(Dashboard)
        setBtnBg(dashSide)

    End Sub


    'VIEW ROOMS BUTTON CLICKED
    Private Sub viewRoomsBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles viewRoomsBtn.MouseDown
        selectViewGeneric(PosRoomCheck, posWindows)
    End Sub

    'BOOK BUTTON
    Private Sub bookBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles bookBtn.MouseDown
        selectViewGeneric(PosRoomBookingGrid, posWindows)
        roomNameTxtBox.Text = selectedRoom.Name
        roomTypeTxtBox.Text = selectedRoom.Type
        roomOccupancyTxtBox.Text = selectedRoom.Capacity
        roomPriceTxtBox.Text = selectedRoom.Price

        startDate.BlackoutDates.Clear()
        endDate.BlackoutDates.Clear()

        'Blacking out dates the are from selected room bookings
        For Each booking As Booking In selectedRoom.Bookings
            Dim startDateDup As New Date(booking.startDate.Year, booking.startDate.Month, booking.startDate.Day)
            Dim endDateDup As New Date(booking.endDate.Year, booking.endDate.Month, booking.endDate.Day)

            startDate.SelectedDate = Nothing
            endDate.SelectedDate = Nothing


            Dim currentDate As Date = startDateDup
            Do While currentDate <= endDateDup
                startDate.BlackoutDates.Add(New CalendarDateRange(New Date(currentDate.Year, currentDate.Month, currentDate.Day)))
                endDate.BlackoutDates.Add(New CalendarDateRange(New Date(currentDate.Year, currentDate.Month, currentDate.Day)))

                currentDate = currentDate.AddDays(1)
                'MsgBox(currentDate & endDateDup)
                'MsgBox("ror")
            Loop

        Next

    End Sub


    'When date in date picker is changed
    Private Sub selectedDateChangedFunc(sender As Object, e As SelectionChangedEventArgs)

        Dim selectedStartDate As Date? = startDate.SelectedDate
        Dim selectedEndDate As Date? = endDate.SelectedDate

        Dim finalStartDateTime As Date
        Dim finalEndDateTime As Date

        If selectedStartDate.HasValue And selectedEndDate.HasValue Then
            Dim startDate = selectedStartDate.Value
            Dim endDate = selectedEndDate.Value

            Dim startTime = startTimeTxtbox.Text.Trim()
            Dim endTime = endTimeTxtbox.Text.Trim()

            Dim startParsedTime As Date
            Dim endParsedTime As Date



            If Date.TryParse(startTime, startParsedTime) And Date.TryParse(endTime, endParsedTime) Then
                finalStartDateTime = New Date(startDate.Year, startDate.Month, startDate.Day, startParsedTime.Hour, startParsedTime.Minute, 0)
                finalEndDateTime = New Date(endDate.Year, endDate.Month, endDate.Day, endParsedTime.Hour, endParsedTime.Minute, 0)
            End If

        End If


        'Calculating for the difference of days from starting to end
        Dim difference As TimeSpan = finalEndDateTime - finalStartDateTime
        Dim daysGap As Integer = difference.Days
        Dim hoursGap As Integer = difference.TotalHours

        totalDaysTxtBox.Text = daysGap
        amountPayTxtBox.Text = daysGap * selectedRoom.Price




    End Sub

    'CLEAR POS
    Sub clearPos()
        customerNameTxtBox.Clear()
        customerNumberTxtBox.Clear()
        customerEmailTxtBox.Clear()
        partySizeTxtBox.Clear()
        paymentTxtBox.Clear()
        If Not startDate.BlackoutDates.Contains(New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day)) Then
            startDate.SelectedDate = Date.Now
        End If
        startTimeTxtbox.Text = "12:00"
        endDate.SelectedDate = Nothing
        endTimeTxtbox.Text = "12:00"
        roomNameTxtBox.Clear()
        roomTypeTxtBox.Clear()
        roomOccupancyTxtBox.Clear()
        roomPriceTxtBox.Clear()
        totalDaysTxtBox.Clear()
        amountPayTxtBox.Clear()
        paymentTxtBox.Clear()

        selectViewGeneric(PosRoomCheck, posWindows)
    End Sub

    'CONFIRM BOOK BUTTON
    Private Sub confirmBookBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles confirmBookBtn.MouseDown
        Dim selectedStartDate As Date? = startDate.SelectedDate
        Dim selectedEndDate As Date? = endDate.SelectedDate

        Dim finalStartDateTime As Date
        Dim finalEndDateTime As Date

        If selectedStartDate.HasValue And selectedEndDate.HasValue Then
            Dim startDate = selectedStartDate.Value
            Dim endDate = selectedEndDate.Value

            Dim startTime = startTimeTxtbox.Text.Trim()
            Dim endTime = endTimeTxtbox.Text.Trim()

            Dim startParsedTime As Date
            Dim endParsedTime As Date



            If Date.TryParse(startTime, startParsedTime) And Date.TryParse(endTime, endParsedTime) Then
                finalStartDateTime = New Date(startDate.Year, startDate.Month, startDate.Day, startParsedTime.Hour, startParsedTime.Minute, 0)
                finalEndDateTime = New Date(endDate.Year, endDate.Month, endDate.Day, endParsedTime.Hour, endParsedTime.Minute, 0)
            End If

            Dim name = customerNameTxtBox.Text
            Dim contactNumber = Val(customerNumberTxtBox.Text)
            Dim email = customerEmailTxtBox.Text
            Dim partySize = Val(partySizeTxtBox.Text)
            Dim payment = Val(paymentTxtBox.Text)

            'condition TO Check if the booking if overlapping other bookings


            selectedRoom.Bookings.Add(New Booking(selectedRoom.Name, selectedRoom.Type, name, contactNumber, email, partySize, payment, finalStartDateTime, finalEndDateTime))
            'selectedRoom.Active = False
            clearPos()
            bookingTimer_Tick()

        End If


    End Sub



    'Generic horizontal scroll wheel function for ListBoxes
    Private Sub HorizontalListBox_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        Dim listBox As ListBox = TryCast(sender, ListBox)

        If listBox IsNot Nothing Then
            Dim scrollViewer As ScrollViewer = FindVisualChild(Of ScrollViewer)(listBox)

            If scrollViewer IsNot Nothing Then
                If e.Delta < 0 Then
                    scrollViewer.LineRight()
                Else
                    scrollViewer.LineLeft()
                End If
                e.Handled = True ' Stop normal vertical scrolling
            End If
        End If
    End Sub


    ' Helper to find ScrollViewer inside the ListBox / Function helper for horizontal scrolling
    Private Function FindVisualChild(Of T As DependencyObject)(obj As DependencyObject) As T
        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
            Dim child As DependencyObject = VisualTreeHelper.GetChild(obj, i)
            If child IsNot Nothing AndAlso TypeOf child Is T Then
                Return CType(child, T)
            Else
                Dim childOfChild As T = FindVisualChild(Of T)(child)
                If childOfChild IsNot Nothing Then
                    Return childOfChild
                End If
            End If
        Next
        Return Nothing
    End Function

    'View Room Function - when a room is clicked
    Private Sub ViewRoom(sender As Object, e As MouseButtonEventArgs)
        selectViewGeneric(PosRoomDetailsGrid, posWindows)

        'Get the ui element that was clicked
        Dim clickedElement As DependencyObject = TryCast(sender, DependencyObject)

        If clickedElement IsNot Nothing Then
            'Traverse up the visual tree to find the listboxItem
            Dim listBoxItem As ListBoxItem = TryFindAncestor(Of ListBoxItem)(clickedElement)

            If listBoxItem IsNot Nothing Then
                Dim room As Room = TryCast(listBoxItem.DataContext, Room)

                If room IsNot Nothing Then
                    selectedRoom = room

                    roomName.Content = "Room: " & selectedRoom.Name
                    roomType.Content = "Type: " & selectedRoom.Type
                    roomCapacity.Content = "Capacity: " & selectedRoom.Capacity
                    roomPrice.Content = "Price/Night: " & selectedRoom.Price
                    roomStatus.DataContext = selectedRoom
                    'roomStatus.Content = selectedRoom.statusText
                    'roomStatus.Foreground = CType(New BrushConverter().ConvertFromString(selectedRoom.getStatusColor), Brush)
                    'selectedRoom.checkStatus(New Date.Today)
                    roomFeatures.ItemsSource = selectedRoom.Features
                    roomBookings.ItemsSource = selectedRoom.Bookings

                End If

            End If

        End If

    End Sub

    'Helper Function for finding ancestor in visual tree listbox
    Private Function TryFindAncestor(Of T As DependencyObject)(depObj As DependencyObject) As T
        Dim parent As DependencyObject = VisualTreeHelper.GetParent(depObj)

        While parent IsNot Nothing AndAlso Not TypeOf parent Is T
            parent = VisualTreeHelper.GetParent(parent)
        End While

        Return TryCast(parent, T)
    End Function


    'When calendar changes displays the bookings based on the Date Selected
    Private Sub calendarBox_SelectedDatesChanged(sender As Object, e As SelectionChangedEventArgs) Handles calendarBox.SelectedDatesChanged

        getRooms()
        getBookings()

        calndarBookings.ItemsSource = getBookingsOnDate(calendarBox.SelectedDate)

    End Sub
End Class
