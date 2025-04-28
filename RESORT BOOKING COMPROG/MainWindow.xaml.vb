Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Class MainWindow
    Implements INotifyPropertyChanged

    'VARIABLES
    'PAGES
    Dim pages As New List(Of String) From {"login", "main"}

    'MAIN PAGE VIEWS
    Dim views As New List(Of Grid) From {POS, Calendar, Dashboard, Room, Security}

    'NAV BUTTONS
    Dim navBtns As New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}

    'POS WINDOWS
    Dim posWindows As New List(Of Grid) From {PosRoomCheck, PosRoomDetailsGrid, PosRoomBookingGrid}

    'ROOMS
    Dim rooms As New ObservableCollection(Of Room)

    'ROOM TYPES
    Dim roomTypes As New ObservableCollection(Of RoomType)




    'VARIABLE SELECTIONS
    'Public Property selectedRoom As Room
    Private selectedRoom As Room


    'HAYST EWAN KO TOH
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Me
        selectedRoom = New Room("ojnanw", "ojbae", 1, 0)

    End Sub



    'PRORTY CHANGE
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    'FUNCTIONS
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

        roomTypes.Add(regular)
        roomTypes.Add(premium)
        roomTypes.Add(deluxe)

        'Assigning Roomtypes as item source for RoomTypeListBox
        RoomTypeListBox.ItemsSource = roomTypes


        'Assigning Starting Dates for POS date picker
        startDate.SelectedDate = Date.Now


    End Sub

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

            selectedRoom.Bookings.Add(New Booking(finalStartDateTime, finalEndDateTime))
        End If


        'Calculating for the difference of days from starting to end
        Dim difference As TimeSpan = finalEndDateTime - finalStartDateTime
        Dim daysGap As Integer = difference.Days
        Dim hoursGap As Integer = difference.TotalHours

        totalDaysTxtBox.Text = daysGap
        amountPayTxtBox.Text = daysGap * selectedRoom.Price




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

            selectedRoom.Bookings.Add(New Booking(finalStartDateTime, finalEndDateTime))
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
                    If selectedRoom.Active Then
                        roomStatus.Content = "Available"
                        roomStatus.Foreground = New SolidColorBrush(Colors.Green)
                    Else
                        roomStatus.Content = "Not Available"
                        roomStatus.Foreground = New SolidColorBrush(Colors.Red)
                    End If

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


End Class
