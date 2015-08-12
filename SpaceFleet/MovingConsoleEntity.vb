Public Class MovingConsoleEntity
    Implements IConsoleEntity, IColourful

    Protected Ships As List(Of Ship)
    Public Owner As Player
    Protected FleetNumber As Integer

    Public Destination As Integer

    Private MyLocation As Integer
    Public Property Location As Integer Implements IConsoleEntity.Location
        Get
            Return MyLocation
        End Get
        Set(value As Integer)
            MyLocation = value
        End Set
    End Property

    Protected MyName As String
    Public Property Name As String Implements IConsoleEntity.Name
        Get
            Return MyName
        End Get
        Set(value As String)
            MyName = value
        End Set
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Ships.Count
        End Get
    End Property

    Public ReadOnly Property Moving As Boolean
        Get
            Return Location <> Destination
        End Get
    End Property

    Public Sub New()
        Me.Name = ""
    End Sub

    Public Sub Draw() Implements IConsoleEntity.Draw

        Dim LocationString As String = ""
        If Moving() Then
            'TODO show number of weeks left in transport
            LocationString = String.Format("{0}pc -> {1}pc", Me.Location, Me.Destination)
        Else
            LocationString = String.Format("{0}pc", Me.Location)
        End If

        If TypeOf Me.Owner Is Human Then
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = ConsoleColor.Black
        Else
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = Me.Owner.Race.Colour
        End If

        Console.Write("{0}{1}{2}", Me.Art(), vbTab, Me.Name)

        'Reset console defaults
        ResetConsole()

        Console.Write(" {0}pc", Me.Location)

        'Write destination if the ship has belongs to the player
        If Me.Destination <> Me.Location _
            AndAlso TypeOf Me.Owner Is Human Then
            Console.Write(" -> {0}pc", Me.Destination)
        End If

        'End the line
        Console.WriteLine()

    End Sub

    Public Sub WriteName() Implements IColourful.WriteName

        If TypeOf Me.Owner Is Human Then
            ResetConsole()
        Else
            Console.ForegroundColor = Me.Owner.Race.Colour
        End If

        Console.Write(Me.Name)

        'Reset console defaults
        ResetConsole()

    End Sub

End Class
