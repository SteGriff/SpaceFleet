Public MustInherit Class MobileEntity
    Implements IConsoleEntity, IColourful

    Overridable ReadOnly Property Art As String
        Get
            Return "£"
        End Get
    End Property

    Overridable ReadOnly Property Warp As Integer
        Get
            Return 0
        End Get
    End Property

    MustOverride ReadOnly Property ShipContent As List(Of Ship)

    Protected FleetNumber As Integer

    Public Owner As Player

    Public Destination As Integer
    Public Engaged As Boolean

    'Has this entity's movement been processed this turn?
    Public Moved As Boolean

    Private OldLocation As Integer

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

    Public Function IsEnemy(S As MobileEntity) As Boolean

        Return S.Owner.Team <> Me.Owner.Team

    End Function

    Public Function IsTeammate(S As MobileEntity) As Boolean

        Return S.Owner.Team = Me.Owner.Team

    End Function

    Public Function Move(AllShips As List(Of MobileEntity)) As CollectionChangeStatus

        'Whether or not we actually move, don't process this ship again
        Me.Moved = True

        'Can't move if in battle
        If Engaged Then
            Return CollectionChangeStatus.None
        End If

        OldLocation = Location

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

        'Check ship interactions - did we pass another ship? Shall we fleet/battle?
        'There will be unprocessed ships if the AllShips collection had to change
        ' i.e. if there was a battle or fleet change
        Dim ChangeStatus = ProcessAllInteractions(AllShips, AllShips)
        Return ChangeStatus

    End Function

    ''' <summary>
    ''' Recursive function to process interactions with some subset of ships
    ''' </summary>
    ''' <param name="SomeShips">Ships to potentially interact with</param>
    ''' <param name="AllShips">All ships in the universe</param>
    Private Function ProcessAllInteractions(SomeShips As List(Of MobileEntity), AllShips As List(Of MobileEntity)) As CollectionChangeStatus

        Dim ChangeStatus As CollectionChangeStatus = CollectionChangeStatus.None

        'Clone AllShips into UnprocessedShips, but remove them
        ' when they are later processed
        Dim UnprocessedShips As New List(Of MobileEntity)(SomeShips)

        For Each E As MobileEntity In SomeShips

            UnprocessedShips.Remove(E)
            Dim ForState = ProcessEntityInteractions(E, AllShips)

            Select Case ForState
                Case ForLoopTransition.ContinueFor
                    Continue For
                Case ForLoopTransition.ExitFor
                    ChangeStatus = CollectionChangeStatus.Changed
                    Exit For
            End Select
        Next

        'Base case is we're finished and there are no unprocessed ships

        'Any unprocessed, run the function on the subset
        If UnprocessedShips.Count > 0 Then
            'Recursive call, passing in the as-yet unprocessed ships
            ProcessAllInteractions(UnprocessedShips, AllShips)
        End If

        Return ChangeStatus

    End Function

    Private Function ProcessEntityInteractions(E As MobileEntity, AllShips As List(Of MobileEntity)) As ForLoopTransition

        'Don't interact with yourself
        If E.Equals(Me) Then
            Return ForLoopTransition.ContinueFor
        End If

        'Check collisions
        If E.IsBetween(OldLocation, Location) Then

            If IsEnemy(E) Then
                'Stop at that location and engage in battle
                Me.Location = E.Location
                Me.Engaged = True
                E.Engaged = True
                Return ForLoopTransition.ExitFor

            ElseIf Location = Destination AndAlso _
                Location = E.Location AndAlso _
                IsTeammate(E) Then

                'Not an enemy and we have stopped on the same spot as the other guy
                'They're allies - fleet up
                'Does fleet already exist?
                If TypeOf E Is Fleet Then
                    'Join existing fleet
                    DirectCast(Me, Ship).JoinFleet(E)
                    'DirectCast(E, Fleet).AssembleFleet(E.Owner, E.Location, AllShips)

                ElseIf TypeOf Me Is Fleet Then
                    DirectCast(Me, Fleet).AssembleFleet(Me.Owner, Me.Location, AllShips)

                ElseIf TypeOf Me Is Ship And DirectCast(Me, Ship).Fleet Is Nothing Then
                    'No existing fleet, so form one
                    Dim F As New Fleet(Me.Owner, Me.Location, AllShips)
                    Return ForLoopTransition.ExitFor
                End If
            End If

        End If

        Return ForLoopTransition.NextFor

    End Function

    Private Enum ForLoopTransition
        NextFor
        ContinueFor
        ExitFor
    End Enum

    Public Enum CollectionChangeStatus
        None
        Changed
    End Enum

End Class
