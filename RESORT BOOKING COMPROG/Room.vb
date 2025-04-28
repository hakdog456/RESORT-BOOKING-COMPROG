Imports System.Runtime.InteropServices

Public Class Room

    Public Property Name As String
    Public Property Type As String
    Public Property Capacity As Integer
    Public Property Price As Double
    Public Property Active As Boolean = True
    Public Property Bookings As List(Of Booking)
    Public Property Features As List(Of String)
    Public Property Pictures As List(Of Object)
    Public Property Color As String


    'CONSTRUCTOR
    Sub New(
           name As String,
           type As String,
           capacity As Integer,
           price As Double
           )

        Me.Name = name
        Me.Type = type
        Me.Capacity = capacity
        Me.Price = price

    End Sub

    Public Function statusColor()
        Dim color As String
        If (Active) Then
            color = "#FFA1E6B2"
        Else
            color = "#FFF57879"
        End If

        Return color
    End Function

    'Property that calls my function
    Public ReadOnly Property getStatusColor As String
        Get
            Return statusColor()
        End Get
    End Property








End Class
