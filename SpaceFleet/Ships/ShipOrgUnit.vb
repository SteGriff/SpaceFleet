''' <summary>
''' An organisational unit for 1 or more ships.
''' </summary>
''' <remarks></remarks>
Public Class ShipOrgUnit
    Implements IConsoleEntity, IDisposable

    Public Ships As List(Of Ship)
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

    Public ReadOnly Property Size As Integer
        Get
            Return Ships.Count
        End Get
    End Property

    Public ReadOnly Property IsFleet As Boolean
        Get
            Return Ships.Count > 1
        End Get
    End Property

    Overridable ReadOnly Property Art As String
        Get
            Select Case Me.Size
                Case 0
                    Throw New ApplicationException("0 ships in Fleet")
                Case 1
                    Return Ships.Single().Art
                Case Else
                    Return "» " & Me.Size
            End Select
        End Get
    End Property

    ReadOnly Property Warp As Integer
        Get
            Select Case Me.Size
                Case 0
                    Throw New ApplicationException("0 ships in Fleet")
                Case 1
                    Return Ships.Single().Warp
                Case Else
                    'Warp of the fleet = warp of the slowest ship
                    Return Ships.Min(Function(s) (s.Warp))
            End Select
        End Get
    End Property

    Protected FleetName As String
    Public ReadOnly Property Name As String Implements IConsoleEntity.Name
        Get
            Select Case Me.Size
                Case 0
                    Throw New ApplicationException("0 ships in Fleet")
                Case 1
                    Return Ships.Single().Name
                Case Else
                    Return FleetName
            End Select
        End Get
    End Property

    Public ReadOnly Property Moving As Boolean
        Get
            Return Location <> Destination
        End Get
    End Property

    ''' <summary>
    ''' Newly built ship based on a completed design
    ''' </summary>
    Public Sub New(Owner As Player, Design As Ship, AllShips As List(Of ShipOrgUnit))

        Me.Owner = Owner
        Me.Location = Owner.ConstructionPlanet.Location

        Dim NewShip As Ship = Design.Clone()

        AssignFleetInfo()
        Associate(NewShip, AllShips)

    End Sub

    ''' <summary>
    ''' Create org as branch off from previous fleet org
    ''' </summary>
    Public Sub New(PreviousOrg As ShipOrgUnit, Ship As Ship, AllShips As List(Of ShipOrgUnit))

        Me.Owner = PreviousOrg.Owner
        Me.Location = PreviousOrg.Location
        Me.Destination = PreviousOrg.Destination

        AssignFleetInfo()
        Associate(Ship, AllShips)

    End Sub

    Private Sub Associate(Ship As Ship, AllShips As List(Of ShipOrgUnit))

        If Me.Ships Is Nothing Then
            Me.Ships = New List(Of Ship)
        End If

        'Put the ship in my roster
        Me.Ships.Add(Ship)

        'Set the ships unit to me
        Ship.OrgUnit = Me

        'Add me to the Player's list of units
        Me.Owner.ShipOrgs.Add(Me)

        'Add me to the global register
        AllShips.Add(Me)

    End Sub

    Private Sub AssignFleetInfo()
        Owner.TopFleetNumber += 1
        FleetNumber = Me.Owner.TopFleetNumber
        FleetName = String.Format("{0} fleet {1}", Owner.Race.Name, FleetNumber)
    End Sub

    Public Sub Draw() Implements IConsoleEntity.Draw

        Dim LocationString As String = ""
        If Moving() Then
            'TODO show number of weeks left in transport
            LocationString = String.Format("{0}sm -> {1}sm", Me.Location, Me.Destination)
        Else
            LocationString = String.Format("{0}sm", Me.Location)
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

        Console.Write(" {0}sm", Me.Location)

        'Write destination if the ship has belongs to the player
        If Me.Destination <> Me.Location _
            AndAlso TypeOf Me.Owner Is Human Then
            Console.Write(" -> {0}sm", Me.Destination)
        End If

        'End the line
        Console.WriteLine()

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

    Public Function IsEnemy(S As ShipOrgUnit) As Boolean

        Return S.Owner.Team <> Me.Owner.Team

    End Function

    Public Function IsTeammate(S As ShipOrgUnit) As Boolean

        Return S.Owner.Team = Me.Owner.Team

    End Function

    Public Function Move(AllShips As List(Of ShipOrgUnit)) As CollectionChangeStatus

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

        Debug.WriteLineIf(Location < 50, String.Format("{0} moved from {1} to {2}sm", Me.Name, OldLocation, Location))

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
    Private Function ProcessAllInteractions(SomeShips As List(Of ShipOrgUnit), AllShips As List(Of ShipOrgUnit)) As CollectionChangeStatus

        Dim ChangeStatus As CollectionChangeStatus = CollectionChangeStatus.None

        'Clone AllShips into UnprocessedShips, but remove them
        ' when they are later processed
        Dim UnprocessedShips As New List(Of ShipOrgUnit)(SomeShips)

        For Each E As ShipOrgUnit In SomeShips

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

    Private Function ProcessEntityInteractions(E As ShipOrgUnit, AllShips As List(Of ShipOrgUnit)) As ForLoopTransition

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

                'We have stopped on the same spot as an allied ship
                ' Join her fleet
                E.Absorb(Me, AllShips)
                NewReportMessages.Add(String.Format("{0} merged into {1}", Me.Name, E.Name))
                Return ForLoopTransition.ExitFor

            End If

        End If

        Return ForLoopTransition.NextFor

    End Function

    Private Sub Absorb(Ally As ShipOrgUnit, AllShips As List(Of ShipOrgUnit))

        'Take all the ships from the other org
        Me.Ships.AddRange(Ally.Ships)

        AllShips.Remove(Ally)

    End Sub

    Public Sub RemoveShip(aShip As Ship, AllShips As List(Of ShipOrgUnit))

        Me.Ships.Remove(aShip)

        'Nothing left, destroy me
        If Me.Size = 0 Then
            AllShips.Remove(Me)
            Owner.ShipOrgs.Remove(Me)
            Me.Dispose()
        End If

    End Sub

    Public Sub DisbandFleet(AllShips As List(Of ShipOrgUnit))

        'Remove ships until only one left in this org
        Do While Me.Size > 1

            Dim ShipIterator = Ships.GetEnumerator()
            ShipIterator.MoveNext()

            Dim S = ShipIterator.Current

            'Disassociate from me
            Me.Ships.Remove(S)

            'Create new unit and associate the ship with that
            Dim YourNewOrg As ShipOrgUnit = New ShipOrgUnit(Me, S, AllShips)

        Loop

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        'TODO
    End Sub

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
