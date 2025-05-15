Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports MS.Internal.Text.TextInterface
Imports Microsoft.Win32
Imports System.Windows.Media.Imaging
Imports System.IO
Imports System.Data.SQLite
Imports System.Data
Imports System.Data.Entity.ModelConfiguration.Configuration
Imports System.Globalization
Imports System.Windows.Interop
Imports System.Data.Entity.Migrations.Model



Class MainWindow
    Implements INotifyPropertyChanged

    'INITIALIZING DATABASE
    Dim connString As String = "Data source=resortBookingDb.db;Version=3"
    Dim connection As New SQLiteConnection(connString)
    Dim command As New SQLiteCommand("", connection)



    'VARIABLES
    'PAGES
    Public Property pages As New List(Of String) From {"login", "main"}

    'MAIN PAGE VIEWS
    Dim views As New List(Of Grid) From {Promos, POS, Calendar, Dashboard, Room, Security}

    'NAV BUTTONS
    Dim navBtns As New List(Of Border) From {posSide, calSide, dashSide, roomSide, secSide}

    'POS WINDOWS
    Dim posWindows As New List(Of Grid) From {PosRoomCheck, PosRoomDetailsGrid, PosRoomBookingGrid}

    'ROOM MANAGEMENT WINDOWS
    Dim roomWinndows As New List(Of Grid) From {roomManagementEditRoomType, roomManagementAddRoom, roomManagementRoomList, roomManagementAddRoomType, roomManagementEditRoom}

    'BOOKINGS
    Dim bookings As New List(Of Booking) From {}

    'ROOMS
    Dim rooms As List(Of Room)

    'ROOM TYPES
    Dim roomTypes As New ObservableCollection(Of RoomType)

    'PROMOS
    Dim PromosList As New ObservableCollection(Of Promo)

    'PROMOS APPLIED
    Dim PromosAppliedList As New ObservableCollection(Of Promo)


    'ADD ROOM IMAGES
    Dim images As New ObservableCollection(Of ImageSource)

    'EDIT ROOM IMAGES
    Dim editRoomImages As New ObservableCollection(Of ImageSource)

    'EDIT ROOM FEATURES
    Dim editRoomFeaturesList As New ObservableCollection(Of String)

    'ADD ROOM FEATURES
    Dim features As New ObservableCollection(Of String)

    'ADD ROOM TYPE FEATURES
    Dim roomTypeFeatures As New ObservableCollection(Of String)

    'EDIT ROOMTYPE FEATURES
    Dim editRoomTypeFeaturesList As New ObservableCollection(Of String)



    'Timers
    Private bookingTimer As DispatcherTimer

    'VARIABLE SELECTIONS
    'Public Property selectedRoom As Room
    Private selectedRoom As Room
    Private currentBookingViewed As Booking
    Private selectedRoomType As RoomType
    Public selectedRoomToEdit As Room
    Public selectedRoomTypeToEdit As RoomType

    'INITIALIZATIONS
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Me
        selectedRoom = New Room("initial", "sampleType", 0, 0, New RoomType("sample", 0, 0))


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

    'Remove a Booking from a room and transfer to database trash
    Public Sub removeBooking(booking As Booking)
        For Each roomType As RoomType In roomTypes
            If roomType Is booking.room.roomType Then
                For Each room As Room In roomType.Rooms
                    If room Is booking.room Then
                        room.removeBooking(booking)
                        removeBookingFromDb(booking)
                        bookingTimer_Tick()
                        calendarBookings.ItemsSource = getBookingsOnDate(calendarBox.SelectedDate)
                    End If

                Next

            End If

        Next

    End Sub


    'GET ALL ROOMS
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
                border.Background = New SolidColorBrush((ColorConverter.ConvertFromString("#FF0A6464")))

            Else
                border.Background = New SolidColorBrush((ColorConverter.ConvertFromString("#FF242222")))
            End If
        Next
    End Sub

    'FIND ROOMTYPE BY ROOMTYPE ID
    Function findRoomTypeById(roomTypeId As String)
        Dim result As RoomType

        For Each roomType As RoomType In roomTypes
            If roomType.id = roomTypeId Then
                result = roomType
            End If
        Next

        Return result
    End Function

    'FIND ROOM BY ROOMTYPE ID
    Function findRoomById(roomId As String, roomType As RoomType)
        Dim result As Room

        If roomType Is Nothing Then
            Return Nothing
        End If

        For Each room As Room In roomType.Rooms
            If room.id = roomId Then
                result = room
            End If
        Next

        Return result
    End Function

    'bytearray to image 
    Private Function ByteArrayToImage(data As Byte()) As BitmapImage
        Using ms As New MemoryStream(data)
            Dim img As New BitmapImage()
            img.BeginInit()
            img.StreamSource = ms
            img.CacheOption = BitmapCacheOption.OnLoad
            img.EndInit()
            img.Freeze() ' Optional: allows cross-thread access
            Return img
        End Using
    End Function


    'Add RoomType to DateBase
    Sub addRoomTypeToDb(roomType As RoomType)
        command.Connection = connection

        Dim addRoomType As String = "INSERT INTO RoomType (Name, Id, Color, Price, Capacity, Features) VALUES (@Name, @Id, @Color, @Price, @Capacity, @Features)"
        Using cmd As New SQLiteCommand(addRoomType, connection)
            cmd.Parameters.AddWithValue("@Name", roomType.Name)
            cmd.Parameters.AddWithValue("@Id", roomType.id)
            cmd.Parameters.AddWithValue("@Color", "none")
            cmd.Parameters.AddWithValue("@Price", roomType.Price)
            cmd.Parameters.AddWithValue("@Capacity", roomType.Capacity)
            cmd.Parameters.AddWithValue("@Features", String.Join("|", roomType.features))
            cmd.ExecuteNonQuery()
        End Using

    End Sub


    'Add ROOM to DateBase
    Sub addRoomToDb(room As Room)
        command.Connection = connection

        Dim addRoom As String = "INSERT INTO Room (Name, Type, Id, RoomTypeId, Capacity, Active, Features, StatusText, Price) VALUES (@Name, @Type, @Id, @RoomTypeId, @Capacity, @Active, @Features, @StatusText, @Price)"
        Using cmd As New SQLiteCommand(addRoom, connection)
            cmd.Parameters.AddWithValue("@Name", room.Name)
            cmd.Parameters.AddWithValue("@Type", room.roomType.Name)
            cmd.Parameters.AddWithValue("@Id", room.id)
            cmd.Parameters.AddWithValue("@RoomTypeId", room.roomTypeId)
            cmd.Parameters.AddWithValue("@Capacity", room.Capacity)
            cmd.Parameters.AddWithValue("@Active", Convert.ToInt32(room.Active))
            cmd.Parameters.AddWithValue("@Features", String.Join("|", room.Features))
            cmd.Parameters.AddWithValue("@StatusText", room.statusText)
            cmd.Parameters.AddWithValue("@Price", room.Price)
            cmd.ExecuteNonQuery()
        End Using

    End Sub


    'Update A ROOM to DateBase
    Sub updateRoomToDb(room As Room)
        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If

        Dim updateRoom As String = "
        UPDATE Room SET
            Name = @Name,
            Type = @Type,
            RoomTypeId = @RoomTypeId,
            Capacity = @Capacity,
            Active = @Active,
            Features = @Features,
            StatusText = @StatusText,
            Price = @Price
        WHERE Id = @Id
    "


        Using cmd As New SQLiteCommand(updateRoom, connection)
            cmd.Parameters.AddWithValue("@Name", room.Name)
            cmd.Parameters.AddWithValue("@Type", room.roomType.Name)
            cmd.Parameters.AddWithValue("@RoomTypeId", room.roomTypeId)
            cmd.Parameters.AddWithValue("@Capacity", room.Capacity)
            cmd.Parameters.AddWithValue("@Active", Convert.ToInt32(room.Active))
            cmd.Parameters.AddWithValue("@Features", String.Join("|", room.Features))
            cmd.Parameters.AddWithValue("@StatusText", room.statusText)
            cmd.Parameters.AddWithValue("@Price", room.Price)
            cmd.Parameters.AddWithValue("@Id", room.id)

            cmd.ExecuteNonQuery()
        End Using

    End Sub

    'Update A ROOM to DateBase
    Sub updateRoomTypeToDb(roomType As RoomType)
        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If

        Dim updateRoom As String = "
        UPDATE RoomType SET
            Name = @Name,
            Capacity = @Capacity,
            Features = @Features,
            Price = @Price
        WHERE Id = @Id
    "
        Using cmd As New SQLiteCommand(updateRoom, connection)
            cmd.Parameters.AddWithValue("@Name", roomType.Name)
            cmd.Parameters.AddWithValue("@Capacity", roomType.Capacity)
            cmd.Parameters.AddWithValue("@Features", String.Join("|", roomType.features))
            cmd.Parameters.AddWithValue("@Price", roomType.Price)
            cmd.Parameters.AddWithValue("@Id", roomType.id)

            cmd.ExecuteNonQuery()
        End Using

    End Sub



    'Remove Room to Data Base
    Sub removeItemInDb(id As String, table As String)
        Dim deleteQuery As String = "DELETE FROM " & table & " WHERE Id = @Id"

        Using cmd As New SQLiteCommand(deleteQuery, connection)
            cmd.Parameters.AddWithValue("@Id", id)
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    'Add Booking to database
    Sub addBookingToDb(booking As Booking)
        Dim addBooking As String = "INSERT INTO Booking (StartDate, EndDate, Name, PartySize, Payment, ContactNumber, Email, RoomName, RoomType, Id, RoomId, Days, RoomTypeId) VALUES (@StartDate, @EndDate, @Name, @PartySize, @Payment, @ContactNumber, @Email, @RoomName, @RoomType, @Id, @RoomId, @Days, @RoomTypeId)"
        Using cmd As New SQLiteCommand(addBooking, connection)
            cmd.Parameters.AddWithValue("@StartDate", booking.startDate)
            cmd.Parameters.AddWithValue("@EndDate", booking.endDate)
            cmd.Parameters.AddWithValue("@Name", booking.name)
            cmd.Parameters.AddWithValue("@PartySize", booking.partySize)
            cmd.Parameters.AddWithValue("@Payment", booking.payment)
            cmd.Parameters.AddWithValue("@ContactNumber", booking.contactNumber)
            cmd.Parameters.AddWithValue("@Email", booking.email)
            cmd.Parameters.AddWithValue("@RoomName", booking.roomName)
            cmd.Parameters.AddWithValue("@RoomType", booking.roomType)
            cmd.Parameters.AddWithValue("@Id", booking.id)
            cmd.Parameters.AddWithValue("@RoomId", selectedRoom.id)
            cmd.Parameters.AddWithValue("@Days", booking.days)
            cmd.Parameters.AddWithValue("@RoomTypeId", selectedRoom.roomTypeId)

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    'remove a booking to DataBase
    Sub removeBookingFromDb(booking As Booking)
        Dim deleteQuery As String = "DELETE FROM Booking WHERE Id = @Id"

        Using cmd As New SQLiteCommand(deleteQuery, connection)
            cmd.Parameters.AddWithValue("@Id", booking.id)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    'bitmap to byteArray
    Function BitmapImageToByteArray(bitmap As BitmapImage) As Byte()
        Dim encoder As New JpegBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(bitmap))

        Using stream As New MemoryStream()
            encoder.Save(stream)
            Return stream.ToArray()
        End Using
    End Function


    'Add Pictures to DataBase
    Sub addPictureToDb(roomPicture As RoomPicture)
        Dim addImage As String = "INSERT INTO RoomPicture (RoomId, imageSource, RoomTypeId, Id) VALUES (@RoomId, @imageSource, @RoomTypeId, @Id)"
        Using cmd As New SQLiteCommand(addImage, connection)
            cmd.Parameters.AddWithValue("@RoomId", roomPicture.roomId)
            cmd.Parameters.AddWithValue("@RoomTypeId", roomPicture.roomTypeId)
            cmd.Parameters.AddWithValue("@Id", roomPicture.id)

            Dim image As Byte() = BitmapImageToByteArray(roomPicture.imageSource)

            Dim imageParam As New SQLiteParameter("@imageSource", DbType.Binary)
            imageParam.Value = image

            cmd.Parameters.Add(imageParam)


            cmd.ExecuteNonQuery()
        End Using
    End Sub


    'deletion in db by ID
    Public Sub DeleteByIdDb(id As String, table As String)
        Dim query As String = "DELETE FROM " & table & " WHERE Id = @Id"

        Using cmd As New SQLiteCommand(query, connection)
            cmd.Parameters.AddWithValue("@Id", id)
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    'Remove removed Pictures in Db
    Sub removePicturesInDb(room As Room)
        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If

        command.Connection = connection
        command.CommandText = "SELECT Id FROM RoomPicture WHERE RoomId = @RoomId"
        Dim query = "SELECT Id FROM RoomPicture WHERE RoomId = @RoomId"

        Dim roomPictureIdsToDelete As New List(Of String) From {}
        Dim roomPictureIdsOfRoom As New List(Of String) From {}

        'gathering current picture Ids of the room
        For Each picture As RoomPicture In room.Pictures
            roomPictureIdsOfRoom.Add(picture.id)
        Next

        'gathering picture Ids from db of the room that are not in the current room
        Using cmd As New SQLiteCommand(query, connection)
            cmd.Parameters.AddWithValue("@RoomId", room.id)

            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim Id As String = reader("Id").ToString()

                    If Not roomPictureIdsOfRoom.Contains(Id) Then
                        roomPictureIdsToDelete.Add(Id)
                    End If

                End While
            End Using
        End Using


        'removing room pictures to delete
        For Each id As String In roomPictureIdsToDelete
            DeleteByIdDb(id, "RoomPicture")
        Next

    End Sub


    'WHEN WINDOW IS LOADED 
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        'Hiding main page
        mainPageGrid.Visibility = Visibility.Collapsed
        PosRoomBookingGrid.Visibility = Visibility.Collapsed
        PosRoomDetailsGrid.Visibility = Visibility.Collapsed

        'Initializing Data Base
        connection.Open()

        'DataBase Connection
        If connection.State = connection.State.Open Then
            MsgBox("connection status: " & connection.State.ToString())

            command.Connection = connection
            command.CommandText = "select * from RoomType"

            'tesring adding to database
            Dim sql As String = "INSERT INTO RoomType DEFAULT VALUES "
            'Using cmd As New SQLiteCommand(sql, connection)
            '    cmd.Parameters.AddWithValue("@Name", "Sample Name")
            '    cmd.Parameters.AddWithValue("@Id", "Sample Id")
            '    cmd.Parameters.AddWithValue("@Color", "Sample Color")
            '    cmd.Parameters.AddWithValue("@Price", 0)
            '    cmd.Parameters.AddWithValue("@Capacity", 0)
            '    cmd.Parameters.AddWithValue("@Features", "Sample Feature")
            '    cmd.ExecuteNonQuery()
            'End Using

            'Getting Room Types
            Dim reader As SQLiteDataReader = command.ExecuteReader
            While reader.Read()
                Dim name = reader.GetString(0)
                Dim Id = reader.GetString(1)
                Dim Color = reader.GetString(2)
                Dim Price = reader.GetDouble(3)
                Dim Capacity = reader.GetInt32(4)
                Dim features = reader.GetString(5)

                MsgBox(name & Id & Color & Price & Capacity & features)
                Dim roomType As New RoomType(name, Capacity, Price)
                roomType.features = features.Split("|"c).ToList()
                roomType.id = Id

                roomTypes.Add(roomType)

            End While
            reader.Close()


            'Getting the rooms
            command.Connection = connection
            command.CommandText = "select * from Room"

            Dim roomReader As SQLiteDataReader = command.ExecuteReader
            While roomReader.Read()
                Dim name = roomReader.GetString(0)
                Dim type = roomReader.GetString(1)
                Dim id = roomReader.GetString(2)
                Dim roomTypeId = roomReader.GetString(3)
                Dim capacity = roomReader.GetInt16(4)
                Dim active = roomReader.GetInt16(5)
                Dim features = roomReader.GetString(6)
                Dim statusText = roomReader.GetString(7)
                Dim price = roomReader.GetDouble(8)

                Dim roomType As RoomType = findRoomTypeById(roomTypeId)

                Dim room As New Room(name, type, capacity, price, roomType)
                room.id = id
                room.roomTypeId = roomTypeId
                room.Active = Convert.ToBoolean(active)
                room.Features = features.Split("|"c).ToList()
                room.statusText = statusText

                roomType.AddRoom(room)

            End While
            roomReader.Close()


            'Setting an initial Booking
            Dim addBooking As String = "INSERT INTO Booking (StartDate, EndDate, Name, PartySize, Payment, ContactNumber, Email, RoomName, RoomType, Id, RoomId, Days) VALUES (@StartDate, @EndDate, @Name, @PartySize, @Payment, @ContactNumber, @Email, @RoomName, @RoomType, @Id, @RoomId, @Days)"
            Using cmd As New SQLiteCommand(addBooking, connection)
                'cmd.Parameters.AddWithValue("@StartDate", Date.Now)
                'cmd.Parameters.AddWithValue("@EndDate", Date.Now.AddDays(4))
                'cmd.Parameters.AddWithValue("@Name", "Sample Name")
                'cmd.Parameters.AddWithValue("@PartySize", 3)
                'cmd.Parameters.AddWithValue("@Payment", 2400)
                'cmd.Parameters.AddWithValue("@ContactNumber", "09687488130")
                'cmd.Parameters.AddWithValue("@Email", "dustingualberto@gmail.com")
                'cmd.Parameters.AddWithValue("@RoomName", "R101")
                'cmd.Parameters.AddWithValue("@RoomType", "REGULAR")
                'cmd.Parameters.AddWithValue("@Id", Guid.NewGuid.ToString())
                'cmd.Parameters.AddWithValue("@RoomId", "e0abe550-f1e5-4949-96cc-5cd45d766af4")
                'cmd.Parameters.AddWithValue("@Days", Date.Now.AddDays(4) - Date.Now)

                'cmd.ExecuteNonQuery()
            End Using


            'Getting the Bookings
            command.Connection = connection
            command.CommandText = "select * from Booking"

            Dim bookingReader As SQLiteDataReader = command.ExecuteReader
            While bookingReader.Read()
                Dim startDate As Date = Date.Parse(bookingReader.GetString(0))
                Dim endDate As Date = Date.Parse(bookingReader.GetString(1))
                Dim name = bookingReader.GetString(2)
                Dim partySize = bookingReader.GetInt64(3)
                Dim payment = bookingReader.GetDouble(4)
                Dim contactNumber = bookingReader.GetString(5)
                Dim email = bookingReader.GetString(6)
                Dim roomName = bookingReader.GetString(7)
                Dim roomType = bookingReader.GetString(8)
                Dim id = bookingReader.GetString(9)
                Dim roomId = bookingReader.GetString(10)
                Dim days = bookingReader.GetInt64(11)
                Dim roomTypeId = bookingReader.GetString(12)

                Dim roomTypeObj As RoomType = findRoomTypeById(roomTypeId)
                Dim room As Room = findRoomById(roomId, roomTypeObj)

                Dim booking As New Booking(room, days, roomName, roomType, name, contactNumber,
                                               email, partySize, payment, startDate, endDate)
                booking.id = id
                booking.roomId = roomId
                booking.roomTypeId = roomTypeId

                'if the room is deleted then dedlete the booking
                If room Is Nothing Then
                    DeleteByIdDb(id, "Booking")
                Else
                    room.addBooking(booking)
                End If



            End While
            bookingReader.Close()


            'adding initial image
            Dim addImage As String = "INSERT INTO RoomPicture (RoomId, imageSource) VALUES (@RoomId, @imageSource) "
            Using cmd As New SQLiteCommand(addImage, connection)
                'cmd.Parameters.AddWithValue("@RoomId", "e0abe550-f1e5-4949-96cc-5cd45d766af4")

                'Dim image As Byte() = File.ReadAllBytes("C:\Users\DUSTINT\source\repos\RESORT BOOKING COMPROG\RESORT BOOKING COMPROG\ASSETS\test hotel interior.jpg")

                'Dim imageParam As New SQLiteParameter("@imageSource", DbType.Binary)
                'imageParam.Value = image
                'cmd.Parameters.Add(imageParam)

                'cmd.ExecuteNonQuery()
            End Using


            'Getting the Images
            command.Connection = connection
            command.CommandText = "select * from RoomPicture"
            Dim RoomPictureReader As SQLiteDataReader = command.ExecuteReader
            While RoomPictureReader.Read()
                Dim roomId = RoomPictureReader.GetString(0)
                Dim roomTypeId = RoomPictureReader.GetString(2)
                Dim Id = RoomPictureReader.GetString(3)
                Dim imageByte As Byte() = CType(RoomPictureReader("imageSource"), Byte())

                'Convert to bitmap image
                Dim imageSource As BitmapImage = ByteArrayToImage(imageByte)

                Dim roomPicture As New RoomPicture(roomId, roomTypeId, imageSource)
                roomPicture.id = Id
                Dim roomType As RoomType = findRoomTypeById(roomTypeId)
                Dim room As Room = findRoomById(roomId, roomType)

                room.Pictures.Add(roomPicture)

            End While
            RoomPictureReader.Close()

        End If




        'Reinitializing views of grid
        views = New List(Of Grid) From {Promos, POS, Calendar, Dashboard, Room, Security}
        navBtns = New List(Of Border) From {promosSide, posSide, calSide, dashSide, roomSide, secSide}
        posWindows = New List(Of Grid) From {PosRoomCheck, PosRoomDetailsGrid, PosRoomBookingGrid}
        roomWinndows = New List(Of Grid) From {roomManagementEditRoomType, roomManagementAddRoom, roomManagementRoomList, roomManagementAddRoomType, roomManagementEditRoom}


        'selecting the first pages to show
        selectView(POS)
        setBtnBg(posSide)
        selectViewGeneric(roomManagementRoomList, roomWinndows)


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

        Dim kyran As New RoomType("kyran", 2, 5000)
        kyran.AddRoom("K101")
        kyran.AddRoom("K102")
        kyran.AddRoom("K103")
        kyran.AddRoom("K104")


        Dim childPromo As New Promo("Child Promo", "direct", 500, 1000)
        PromosList.Add(childPromo)
        Dim specialPromo As New Promo("Special Promo", "percentage", 10, 1)
        PromosList.Add(specialPromo)


        'ADDING SAMPLE ROOMS
        'roomTypes.Add(regular)
        'roomTypes.Add(premium)
        'roomTypes.Add(deluxe)
        'roomTypes.Add(bembang)
        'roomTypes.Add(kyran)



        'Assigning Roomtypes as item source for RoomTypeListBox
        RoomTypeListBox.ItemsSource = roomTypes

        'Assigning Roomtypes as item source for roomTypePickerListBox
        roomTypePickerListBox.ItemsSource = roomTypes

        'assigning the first item in rooms for the item of data grid in room management
        If roomTypes.Count > 0 Then
            roomTypePickerListBox.SelectedItem = roomTypePickerListBox.Items(0)
            selectedRoomType = roomTypePickerListBox.Items(0)
            roomsDataGrid.ItemsSource = roomTypePickerListBox.SelectedItem.Rooms
        End If

        'Assigning Starting Dates for POS date picker
        startDate.SelectedDate = Date.Now

        'Setting the default selected date for calendar to today
        calendarBox.SelectedDate = Date.Now

        'Assigning item source for combo box in add room
        addRoomTypeInput.ItemsSource = roomTypes

        'Assigning item source for combo box in edit room
        editRoomTypeInput.ItemsSource = roomTypes

        'Assigning item source of AddRoom Add Images 
        addRoomPicturesCon.ItemsSource = images

        'Assigning item source of AddRoom Features
        addRoomFeatures.ItemsSource = features

        'Assigning item source of AddRoomType Features
        addRoomTypeFeatures.ItemsSource = roomTypeFeatures

        'Assigning item source of Promos
        promosCon.ItemsSource = PromosList




    End Sub


    'Get Bookings based on Date
    Public Function getBookingsOnDate(givenDate As Date)
        Dim bookingsOnDate As New List(Of Booking)

        getRooms()
        getBookings()

        For Each booking As Booking In bookings

            Dim startDay As New Date(booking.startDate.Year, booking.startDate.Month, booking.startDate.Day)
            Dim endDay As New Date(booking.endDate.Year, booking.endDate.Month, booking.endDate.Day)

            If givenDate >= startDay AndAlso givenDate <= endDay Then
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
    Private Sub promosSide_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles promosSide.MouseDown
        selectView(Promos)
        setBtnBg(promosSide)
    End Sub

    'PROMOS BUTTON CLICKED
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
        clearPos()
    End Sub

    'BOOK BUTTON - OPEN BOOKING POS VIEW
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


        'showing promos
        bookingPosAvailablePromos.ItemsSource = PromosList
        bookingPosAppliedPromos.ItemsSource = PromosAppliedList

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
                finalStartDateTime = New Date(startDate.Year, startDate.Month, startDate.Day)
                finalEndDateTime = New Date(endDate.Year, endDate.Month, endDate.Day)
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
        PromosAppliedList.Clear()

        selectViewGeneric(PosRoomCheck, posWindows)
    End Sub

    'CONFIRM BOOK BUTTON
    Private Sub confirmBookBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles confirmBookBtn.MouseDown

        Dim selectedStartDate As Date? = startDate.SelectedDate
        Dim selectedEndDate As Date? = endDate.SelectedDate
        Dim startDateOnly As Date

        Dim finalStartDateTime As Date
        Dim finalEndDateTime As Date

        If selectedStartDate.HasValue And selectedEndDate.HasValue Then

            Dim startDateValue = selectedStartDate.Value
            Dim endDateValue = selectedEndDate.Value
            startDateOnly = startDateValue.Date


            Dim startTime = startTimeTxtbox.Text.Trim()
            Dim endTime = endTimeTxtbox.Text.Trim()

            Dim startParsedTime As Date
            Dim endParsedTime As Date

            Dim validBooking As Boolean = True

            Dim errMsg As String

            If Date.TryParse(startTime, startParsedTime) And Date.TryParse(endTime, endParsedTime) Then
                finalStartDateTime = New Date(startDateValue.Year, startDateValue.Month, startDateValue.Day, startParsedTime.Hour, startParsedTime.Minute, 0)
                finalEndDateTime = New Date(endDateValue.Year, endDateValue.Month, endDateValue.Day, endParsedTime.Hour, endParsedTime.Minute, 0)
            End If

            'condition To Check if the booking if overlapping other bookings
            For Each booking As Booking In selectedRoom.Bookings
                If finalStartDateTime <= booking.startDate And finalEndDateTime >= booking.endDate Then
                    validBooking = False
                    errMsg = "Booking Overlapped!, Select a valid Booking"
                End If
            Next

            'condition to check if the booking is not reversed
            If finalEndDateTime <= finalStartDateTime Then
                validBooking = False
                errMsg = "Invalid Booking!, Starting Date is Greater than the End Date"
            End If

            'If the Booking is valid and passed, then continue booking
            If validBooking Then
                Dim name = customerNameTxtBox.Text
                Dim contactNumber = Val(customerNumberTxtBox.Text)
                Dim email = customerEmailTxtBox.Text
                Dim partySize = Val(partySizeTxtBox.Text)
                Dim payment = Val(paymentTxtBox.Text)
                Dim days As Integer = Val(totalDaysTxtBox.Text)

                'Adding the new Booking 
                Dim newBooking As New Booking(selectedRoom, days, selectedRoom.Name, selectedRoom.Type, name, contactNumber, email, partySize, payment, finalStartDateTime, finalEndDateTime)
                newBooking.Promos = PromosAppliedList.ToList()
                selectedRoom.addBooking(newBooking)
                addBookingToDb(newBooking)
                clearPos()
                bookingTimer_Tick()

                newBooking.showReceipt()

                'refresh calendar box list box
                calendarBookings.ItemsSource = getBookingsOnDate(calendarBox.SelectedDate)


            Else
                MsgBox(errMsg)
                startDate.SelectedDate = Nothing
                endDate.SelectedDate = Nothing
            End If

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

    'horizontal scrolling for scrollviwers
    Private Sub HorizontalScrollViewer_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        ' Try to cast sender as ScrollViewer directly
        Dim scrollViewer As ScrollViewer = TryCast(sender, ScrollViewer)

        ' If sender isn't a ScrollViewer, try to find one inside it
        If scrollViewer Is Nothing Then
            scrollViewer = FindVisualChild(Of ScrollViewer)(TryCast(sender, DependencyObject))
        End If

        ' If we found a scroll viewer, scroll it horizontally
        If scrollViewer IsNot Nothing Then
            If e.Delta < 0 Then
                scrollViewer.LineRight()
            Else
                scrollViewer.LineLeft()
            End If
            e.Handled = True
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


    'next and previous of image viewer
    Dim currentIndex = 0
    Private Sub NextButton_Click(sender As Object, e As RoutedEventArgs) Handles NextButton.Click

        If selectedRoom.Pictures.Count = 0 Then Return

        currentIndex = (currentIndex + 1) Mod selectedRoom.Pictures.Count
        ImageViewer.Source = selectedRoom.Pictures(currentIndex).imageSource

    End Sub

    Private Sub PrevButton_Click(sender As Object, e As RoutedEventArgs) Handles PrevButton.Click
        If selectedRoom.Pictures.Count = 0 Then Return

        currentIndex = (currentIndex - 1 + selectedRoom.Pictures.Count) Mod selectedRoom.Pictures.Count
        ImageViewer.Source = selectedRoom.Pictures(currentIndex).imageSource

    End Sub


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

                    If selectedRoom.Pictures.Count > 0 Then
                        ImageViewer.Source = selectedRoom.Pictures(0).imageSource
                    End If

                End If

            End If

        End If

    End Sub

    'when a booking in room details is double clicked 
    Private Sub Label_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim bookingToView = CType(roomBookings.SelectedItem, Booking)
        currentBookingViewed = bookingToView

        Dim bookingDetails As New bookingDetails(Me, bookingToView)
        AddHandler bookingDetails.removeBookingFromMain, AddressOf handleRemoveBooking
        AddHandler bookingDetails.showReceipt, AddressOf handleShowReceipt

        bookingDetails.Show()

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

        If calendarBox.SelectedDate IsNot Nothing Then

            Dim bookingsOnDay = getBookingsOnDate(calendarBox.SelectedDate)

            Dim selectedDate As Date = calendarBox.SelectedDates(0)
            dateSelectedLbl.Content = "- " & selectedDate.ToString("MMMM d, yyyy")


            'CheckBox how many reserved on Occupied Rooms
            Dim reservedRoomsOnDay = 0
            Dim occupiedRoomsOnDay = 0
            Dim dateToday = Date.Now()

            For Each booking As Booking In bookingsOnDay
                If dateToday >= booking.startDate AndAlso dateToday <= booking.endDate Then
                    occupiedRoomsOnDay += 1
                    booking.color = "#FF6587C5"
                Else
                    reservedRoomsOnDay += 1
                    booking.color = "#FF92BD8C"
                End If
            Next

            reservedRoomsPerDayLabel.Content = reservedRoomsOnDay
            occupiedRoomsPerDayLabel.Content = occupiedRoomsOnDay
            calendarBookings.ItemsSource = bookingsOnDay


        End If


    End Sub

    'select today button in calendar
    Private Sub Label_MouseDown(sender As Object, e As MouseButtonEventArgs)
        calendarBox.SelectedDate = Date.Now
        Keyboard.ClearFocus()


    End Sub

    'View Booking Button on calendar, shows Booking details window launches booking details window
    Private Sub viewBookingBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles viewBookingBtn.MouseDown
        Dim bookingToView = CType(calendarBookings.SelectedItem, Booking)
        currentBookingViewed = bookingToView

        If currentBookingViewed IsNot Nothing Then
            Dim bookingDetails As New bookingDetails(Me, bookingToView)
            AddHandler bookingDetails.removeBookingFromMain, AddressOf handleRemoveBooking
            AddHandler bookingDetails.showReceipt, AddressOf handleShowReceipt

            bookingDetails.Show()
        End If

    End Sub

    'handles when the booking details window initiated a remove Booking
    Sub handleRemoveBooking(sender As Object, e As EventArgs)
        removeBooking(currentBookingViewed)
    End Sub

    Sub handleShowReceipt(sender As Object, e As EventArgs)
        currentBookingViewed.showReceipt()
    End Sub

    'addAddAccount View
    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click

        AddAcount.Visibility = Visibility.Visible
        ManageAccount.Visibility = Visibility.Collapsed
    End Sub

    'Joshua function
    Private Sub btnManage_Click(sender As Object, e As RoutedEventArgs) Handles btnManage.Click
        ManageAccount.Visibility = Visibility.Visible
        AddAcount.Visibility = Visibility.Collapsed

    End Sub

    Dim none As New List(Of Room) From {}

    'room management when a room type is selected
    Private Sub roomTypePickerListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles roomTypePickerListBox.SelectionChanged
        If roomTypePickerListBox.SelectedItem Is Nothing Then
            roomsDataGrid.ItemsSource = none
            Return
        End If

        roomsDataGrid.ItemsSource = roomTypePickerListBox.SelectedItem.Rooms
        selectedRoomType = roomTypePickerListBox.SelectedItem
    End Sub

    'Add Room Button when clicked
    Private Sub addNewRoomBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles addNewRoomBtn.MouseDown
        selectViewGeneric(roomManagementAddRoom, roomWinndows)
        addRoomTypeInput.SelectedItem = selectedRoomType
        addRoomCapacityInput.Text = selectedRoomType.Capacity
        addRoomPriceInput.Text = selectedRoomType.Price
        features.Clear()
        For Each item In selectedRoomType.features
            features.Add(item)
        Next

    End Sub

    'add picture button Function
    Private Sub addRoomAddPictureBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles addRoomAddPictureBtn.MouseDown
        Dim dialog As New OpenFileDialog()
        dialog.Filter = "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp"

        If dialog.ShowDialog = True Then
            Dim imagePath As String = dialog.FileName
            Dim bitmap As New BitmapImage()
            bitmap.BeginInit()
            bitmap.UriSource = New Uri(imagePath, UriKind.Absolute)
            bitmap.CacheOption = BitmapCacheOption.OnLoad
            bitmap.EndInit()

            images.Add(bitmap)
        End If
    End Sub

    'add features button
    Private Sub addFeaturesBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles addFeaturesBtn.MouseDown
        If addFeatureInput.Text IsNot "" Then
            Dim feature As String = "✔" & addFeatureInput.Text
            features.Add(feature)
            addFeatureInput.Clear()

        End If
    End Sub

    'remove feature button
    Sub removeFeature(sender As Object, e As RoutedEventArgs)
        'features.Remove(addRoomFeatures.SelectedItem)

        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        features.Remove(itemToDelete.ToString())
    End Sub


    'remove picture button
    Sub removePicture(sender As Object, e As RoutedEventArgs)
        'features.Remove(addRoomFeatures.SelectedItem)

        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        images.Remove(itemToDelete)
    End Sub

    'Add Room Confirm button click to add a room 
    Private Sub AddRoomBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles AddRoomBtn.MouseDown
        Dim name As String = addRoomNameInput.Text
        Dim Type As String = addRoomTypeInput.Text
        Dim capacity As Integer = Val(addRoomCapacityInput.Text)
        Dim Price As Double = Val(addRoomPriceInput.Text)

        Dim room As New Room(name, Type, capacity, Price, selectedRoomType)
        room.Features = features.ToList()


        Dim pictures As New List(Of RoomPicture)

        For Each image As ImageSource In images
            Dim roomPicture As New RoomPicture(room.id, selectedRoomType.id, image)
            pictures.Add(roomPicture)
            addPictureToDb(roomPicture)

        Next

        room.Pictures = pictures

        selectedRoomType.AddRoom(room)
        addRoomToDb(room)


        addRoomNameInput.Clear()
        addRoomTypeInput.SelectedItem = selectedRoomType
        addRoomCapacityInput.Text = selectedRoomType.Capacity
        addRoomPriceInput.Text = selectedRoomType.Price
        features.Clear()
        images.Clear()

        selectViewGeneric(roomManagementRoomList, roomWinndows)


    End Sub

    'cancel add room button
    Private Sub cancelAddRoomBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles cancelAddRoomBtn.MouseDown
        addRoomNameInput.Clear()
        addRoomTypeInput.SelectedItem = selectedRoomType
        addRoomCapacityInput.Text = selectedRoomType.Capacity
        addRoomPriceInput.Text = selectedRoomType.Price
        features.Clear()
        images.Clear()

        selectViewGeneric(roomManagementRoomList, roomWinndows)

    End Sub

    'when combo box selection of add room changes
    Private Sub addRoomTypeInput_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles addRoomTypeInput.SelectionChanged
        'selectedRoomType = addRoomTypeInput.SelectedItem
        roomTypePickerListBox.SelectedItem = addRoomTypeInput.SelectedItem
    End Sub


    'add new Room type Button
    Private Sub addRoomType_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles addRoomType.MouseDown
        selectViewGeneric(roomManagementAddRoomType, roomWinndows)
    End Sub

    'cancel addRoomType button
    Private Sub cancelAddRoomTypeBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles cancelAddRoomTypeBtn.MouseDown
        selectViewGeneric(roomManagementRoomList, roomWinndows)

    End Sub

    'add Room Type features button
    Private Sub addRoomTypeFeaturesBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles addRoomTypeFeaturesBtn.MouseDown
        If addRoomTypeFeatureInput.Text IsNot "" Then
            Dim feature As String = "✔" & addRoomTypeFeatureInput.Text
            roomTypeFeatures.Add(feature)
            addRoomTypeFeatureInput.Clear()

        End If
    End Sub

    'remove room Type feature button
    Sub removeRoomTypeFeature(sender As Object, e As RoutedEventArgs)

        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        roomTypeFeatures.Remove(itemToDelete.ToString())
    End Sub

    'add room Type Button
    Private Sub AddRoomTypeBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles AddRoomTypeBtn.MouseDown
        Dim name As String = addRoomTypeNameInput.Text
        Dim price As Integer = Val(addRoomTypePriceInput.Text)
        Dim capacity As Integer = Val(addRoomTypeCapacityInput.Text)

        Dim roomType As New RoomType(name, capacity, price)
        roomType.features = roomTypeFeatures.ToList()

        roomTypes.Add(roomType)
        addRoomTypeToDb(roomType)

        addRoomTypeNameInput.Clear()
        addRoomTypeCapacityInput.Clear()
        addRoomTypePriceInput.Clear()
        roomTypeFeatures.Clear()

        selectViewGeneric(roomManagementRoomList, roomWinndows)

    End Sub

    'Edit Room Button to view edit Room view
    Private Sub editRoomBtn_Click(sender As Object, e As RoutedEventArgs)
        selectViewGeneric(roomManagementEditRoom, roomWinndows)
        Dim btn As Button = CType(sender, Button)
        selectedRoomToEdit = CType(btn.DataContext, Room)

        editRoomTitle.Content = "Edit Room " & selectedRoomToEdit.Name
        editRoomNameInput.Text = selectedRoomToEdit.Name
        editRoomTypeInput.SelectedItem = selectedRoomToEdit.roomType
        editRoomCapacityInput.Text = selectedRoomToEdit.Capacity
        editRoomPriceInput.Text = selectedRoomToEdit.Price
        editRoomFeatures.ItemsSource = editRoomFeaturesList

        'getting pictures in the room that is edditing
        For Each picture As RoomPicture In selectedRoomToEdit.Pictures
            editRoomImages.Add(picture.imageSource)
        Next

        'getting features of the room that is editing
        For Each feature As String In selectedRoomToEdit.Features
            editRoomFeaturesList.Add(feature)
        Next


        editRoomPicturesCon.ItemsSource = editRoomImages
        editRoomFeatures.ItemsSource = editRoomFeaturesList


    End Sub

    'removing pictures in roomedit
    Private Sub removePicturesEdit_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        editRoomImages.Remove(itemToDelete)
    End Sub

    'adding pictures in room edit
    Private Sub editRoomAddPictureBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles editRoomAddPictureBtn.MouseDown
        Dim dialog As New OpenFileDialog()
        dialog.Filter = "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp"

        If dialog.ShowDialog = True Then
            Dim imagePath As String = dialog.FileName
            Dim bitmap As New BitmapImage()
            bitmap.BeginInit()
            bitmap.UriSource = New Uri(imagePath, UriKind.Absolute)
            bitmap.CacheOption = BitmapCacheOption.OnLoad
            bitmap.EndInit()

            editRoomImages.Add(bitmap)
        End If
    End Sub

    'add Edit Room features button click
    Private Sub editFeaturesBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles editFeaturesBtn.MouseDown
        If editFeatureInput.Text IsNot "" Then
            Dim feature As String = "✔" & editFeatureInput.Text
            editRoomFeaturesList.Add(feature)
            editFeatureInput.Clear()

        End If

    End Sub


    'remove feature button in room editing
    Private Sub editRoomRemoveFeature_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        editRoomFeaturesList.Remove(itemToDelete.ToString())

    End Sub

    'save changes Edit Room
    Private Sub saveChangesRoomBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles saveChangesRoomBtn.MouseDown
        'creating a new changed copy of the Room
        Dim name As String = editRoomNameInput.Text
        Dim Type As String = editRoomTypeInput.Text
        Dim capacity As Integer = Val(editRoomCapacityInput.Text)
        Dim Price As Double = Val(editRoomPriceInput.Text)

        Dim room As New Room(name, Type, capacity, Price, selectedRoomToEdit.roomType)
        room.Features = editRoomFeaturesList.ToList()
        room.id = selectedRoomToEdit.id
        room.roomTypeId = selectedRoomToEdit.roomTypeId


        Dim pictures As New List(Of RoomPicture)

        'adding room pictures to a pictures list
        For Each image As ImageSource In editRoomImages
            Dim roomPicture As New RoomPicture(room.id, selectedRoomType.id, image)
            pictures.Add(roomPicture)
            addPictureToDb(roomPicture)
        Next
        room.Pictures = pictures

        selectedRoomToEdit.Name = room.Name
        selectedRoomToEdit.Type = room.Type
        selectedRoomToEdit.Capacity = room.Capacity
        selectedRoomToEdit.Price = room.Price
        selectedRoomToEdit.Pictures = room.Pictures
        selectedRoomToEdit.Features = room.Features

        updateRoomToDb(room)

        'checking unused picture references
        removePicturesInDb(room)

        editRoomNameInput.Clear()
        editRoomTypeInput.SelectedItem = Nothing
        editRoomCapacityInput.Clear()
        editRoomPriceInput.Clear()
        editRoomFeaturesList.Clear()
        editRoomImages.Clear()

        selectViewGeneric(roomManagementRoomList, roomWinndows)
        roomsDataGrid.ItemsSource = Nothing
        roomsDataGrid.ItemsSource = selectedRoomType.Rooms

        editRoomImages.Clear()

        RoomTypeListBox.ItemsSource = Nothing
        RoomTypeListBox.ItemsSource = roomTypes

    End Sub

    'Remove Room Button Click
    Private Sub removeRoomBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles removeRoomBtn.MouseDown
        removeItemInDb(selectedRoomToEdit.id, "Room")
        findRoomTypeById(selectedRoomToEdit.roomTypeId).removeRoom(selectedRoomToEdit)

        'remove pictures of room
        selectedRoomToEdit.Pictures.Clear()
        removePicturesInDb(selectedRoomToEdit)

        selectViewGeneric(roomManagementRoomList, roomWinndows)
        roomsDataGrid.ItemsSource = Nothing
        roomsDataGrid.ItemsSource = selectedRoomType.Rooms

        editRoomImages.Clear()
        editRoomFeaturesList.Clear()

    End Sub

    'open edit RoomType
    Private Sub roomTypeBtn_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)

        selectViewGeneric(roomManagementEditRoomType, roomWinndows)
        Dim border As Border = CType(sender, Border)
        Dim getRoomType = CType(border.DataContext, RoomType)

        Dim roomType As RoomType = findRoomTypeById(getRoomType.id)

        selectedRoomTypeToEdit = roomType

        editRoomTypeTitle.Content = "Edit RoomType " & roomType.Name
        editTypeNameInput.Text = roomType.Name
        editRoomTypePriceInput.Text = roomType.Price
        editRoomTypeCapacityInput.Text = roomType.Capacity

        editRoomTypeFeaturesList.Clear()
        For Each feature As String In roomType.features
            editRoomTypeFeaturesList.Add(feature)
        Next

        editRoomTypeFeatures.ItemsSource = editRoomTypeFeaturesList



    End Sub

    'add editRoomTypeFeatures Button
    Private Sub editRoomTypeFeaturesBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles editRoomTypeFeaturesBtn.MouseDown
        If editRoomTypeFeatureInput.Text IsNot "" Then
            Dim feature As String = "✔" & editRoomTypeFeatureInput.Text
            editRoomTypeFeaturesList.Add(feature)
            editRoomTypeFeatureInput.Clear()

        End If

    End Sub


    'save changes Edit RoomType Button
    Private Sub saveChangesRoomTypeBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles saveChangesRoomTypeBtn.MouseDown
        'creating a new changed copy of the Room
        Dim name As String = editTypeNameInput.Text
        Dim capacity As Integer = Val(editRoomTypeCapacityInput.Text)
        Dim Price As Double = Val(editRoomTypePriceInput.Text)

        Dim roomType As New RoomType(name, capacity, Price)
        roomType.features = editRoomTypeFeaturesList.ToList()
        Dim pictures As New List(Of RoomPicture)
        roomType.id = selectedRoomTypeToEdit.id

        'updating the current room type selected
        selectedRoomTypeToEdit.Name = name
        selectedRoomTypeToEdit.Capacity = capacity
        selectedRoomTypeToEdit.Price = Price
        selectedRoomTypeToEdit.features = editRoomTypeFeaturesList.ToList()

        updateRoomTypeToDb(roomType)

        editTypeNameInput.Clear()
        editRoomTypeCapacityInput.Clear()
        editRoomTypePriceInput.Clear()
        editRoomTypeFeaturesList.Clear()

        selectViewGeneric(roomManagementRoomList, roomWinndows)
        roomsDataGrid.ItemsSource = Nothing
        roomsDataGrid.ItemsSource = selectedRoomType.Rooms

        roomTypePickerListBox.ItemsSource = roomTypes.ToList()
        roomTypePickerListBox.ItemsSource = roomTypes

    End Sub

    'remove Feature button
    Private Sub removeRoomTypeFeatureBtn_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim btn As Border = CType(sender, Border)
        Dim itemToDelete = btn.DataContext

        editRoomTypeFeaturesList.Remove(itemToDelete.ToString())

    End Sub

    'remove RoomType button
    Private Sub removeRoomTypeBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles removeRoomTypeBtn.MouseDown

        'removing items from database
        removeItemInDb(selectedRoomTypeToEdit.id, "RoomType")
        For Each room As Room In selectedRoomTypeToEdit.Rooms
            room.Pictures = New List(Of RoomPicture) From {}
            removePicturesInDb(room)

            removeItemInDb(room.id, "Room")
        Next

        roomTypes.Remove(selectedRoomTypeToEdit)


        selectViewGeneric(roomManagementRoomList, roomWinndows)

    End Sub

    Private Sub Border_MouseDown(sender As Object, e As MouseButtonEventArgs)
        MsgBox("rawr")
    End Sub

    'addPromoToApplied 
    Private Sub addPromoToApplied(sender As Object, e As MouseButtonEventArgs)
        Dim border As Border = CType(sender, Border)
        Dim promoSelected = CType(border.DataContext, Promo)

        'new copy of the promo
        Dim promo = New Promo(promoSelected)
        promo.amount = 1
        promo.promoClassId = promoSelected.promoClassId

        Dim meron As Boolean = False

        For Each appliedPromo As Promo In PromosAppliedList
            If promo.promoClassId = appliedPromo.promoClassId Then
                If promoSelected.amount > appliedPromo.amount Then
                    appliedPromo.amount += 1
                Else
                    MsgBox("AMOUNT OF TIMES VOUCHER APPLIED EXCEEDED")
                End If
                meron = True
            End If
        Next

        If Not meron Then
            PromosAppliedList.Add(promo)
        End If

    End Sub

    'minus promo
    Private Sub minusPromo(sender As Object, e As MouseButtonEventArgs)
        Dim border As Border = CType(sender, Border)
        Dim promoSelected = CType(border.DataContext, Promo)

        For Each promoReference As Promo In PromosList
            If promoSelected.promoClassId = promoReference.promoClassId Then
                If promoSelected.amount > 1 Then
                    promoSelected.amount -= 1
                Else
                    PromosAppliedList.Remove(promoSelected)
                End If
            End If
        Next

    End Sub

    'plus promo
    Private Sub plusPromo(sender As Object, e As MouseButtonEventArgs)
        Dim border As Border = CType(sender, Border)
        Dim promoSelected = CType(border.DataContext, Promo)

        For Each promoReference As Promo In PromosList
            If promoSelected.promoClassId = promoReference.promoClassId Then
                If promoSelected.amount < promoReference.amount Then
                    promoSelected.amount += 1
                Else
                    MsgBox("AMOUNT OF TIMES VOUCHER APPLIED EXCEEDED")
                End If
            End If
        Next
    End Sub






    'End of main class
End Class
