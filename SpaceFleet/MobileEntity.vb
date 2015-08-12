Public Class MobileEntity
    Implements IConsoleEntity, IColourful

    Overridable ReadOnly Property Art As String
        Get
            Return "£"
        End Get
    End Property

    Overridable Property Warp As Integer

    Protected FleetNumber As Integer

    Public Owner As Player

    Public Destination As Integer
    Public Engaged As Boolean

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

        Console.Write("{0}{1}{2}", Me.Art, vbTab, Me.Name)

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

    Public Function IsBetween(A As Integer, B As Integer)

        If A > B Then
            Return A >= Me.Location AndAlso Me.Location >= B
        ElseIf A < B Then
            Return A <= Me.Location AndAlso Me.Location <= B
        Else
            'A == B
            Return A = Me.Location
        End If

    End Function

    Public Function IsEnemy(S As Ship) As Boolean

        Return S.Owner.GetType().Name <> Me.Owner.GetType().Name

    End Function

    Public Sub Move(AllShips As List(Of MobileEntity))

        'Can't move if in battle
        If Engaged OrElse Location = Destination Then
            Return
        End If

        Dim OldLocation As Integer = Location

        If Location < Destination Then
            Dim NewLocation As Integer = Location + Warp

            'Correct for overshoot
            If NewLocation > Destination Then
                NewLocation = Destination
            End If

            'Set location
            Location = NewLocation

        ElseIf Location > Destination Then
            Dim NewLocation As Integer = Location - Warp

            'Correct for overshoot
            If NewLocation < Destination Then
                NewLocation = Destination
            End If

            Location = NewLocation

        End If

        Debug.WriteLineIf(Location < 50, String.Format("{0} moved from {1} to {2}pc", Me.Name, OldLocation, Location))

        'Check if we passed another ship
        For Each E As MobileEntity In AllShips

            If E.Equals(Me) Then
                Continue For
            End If

            If E.IsBetween(OldLocation, Location) Then
                If E.IsEnemy(Me) Then
                    'Stop at that location and engage in battle
                    Me.Location = E.Location
                    Me.Engaged = True
                    E.Engaged = True
                    Exit For
                Else
                    'They're allies - fleet up
                    'Does fleet already exist?
                    If TypeOf E Is Fleet Then
                        DirectCast(E, Fleet).AssembleFleet(E.Owner, E.Location, AllShips)
                    ElseIf TypeOf Me Is Fleet Then
                        DirectCast(Me, Fleet).AssembleFleet(Me.Owner, Me.Location, AllShips)
                    End If
                End If

            End If

        Next

    End Sub

End Class
