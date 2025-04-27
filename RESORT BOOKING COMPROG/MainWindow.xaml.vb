Class MainWindow

    'VARIABLES
    'PAGES
    Dim pages As New List(Of String) From {"login", "main"}

    'MAIN PAGE VIEWS
    Dim views As New List(Of Grid) From {POS, Calendar, Dashboard, Room, Security}

    'SELECTED PAGE
    Dim activePage As String = "login"

    'NAV BUTTONS
    Dim navBtns As New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}





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

        'Reinitializing views of grid
        views = New List(Of Grid) From {POS, Calendar, Dashboard, Room, Security}
        navBtns = New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}

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


    'POS BUTTON CLICKED



    'HOrizontal scroll wheel function of REGULAR ROOMS 
    Private Sub lstTest_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        Dim scrollViewer As ScrollViewer = FindVisualChild(Of ScrollViewer)(RegularRoomsListBox)

        If scrollViewer IsNot Nothing Then
            If e.Delta < 0 Then
                scrollViewer.LineRight()
            Else
                scrollViewer.LineLeft()
            End If
            e.Handled = True ' Stop normal vertical scrolling
        End If
    End Sub

    'HOrizontal scroll wheel function of REGULAR ROOMS 
    Private Sub Premium_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        Dim scrollViewer As ScrollViewer = FindVisualChild(Of ScrollViewer)(PremiumRoomsListBox)

        If scrollViewer IsNot Nothing Then
            If e.Delta < 0 Then
                scrollViewer.LineRight()
            Else
                scrollViewer.LineLeft()
            End If
            e.Handled = True ' Stop normal vertical scrolling
        End If
    End Sub

    'HOrizontal scroll wheel function of REGULAR ROOMS 
    Private Sub Deluxe_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        Dim scrollViewer As ScrollViewer = FindVisualChild(Of ScrollViewer)(DeluxeRoomsListBox)

        If scrollViewer IsNot Nothing Then
            If e.Delta < 0 Then
                scrollViewer.LineRight()
            Else
                scrollViewer.LineLeft()
            End If
            e.Handled = True ' Stop normal vertical scrolling
        End If
    End Sub


    ' Helper to find ScrollViewer inside the ListBox
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


End Class
