Public Class SpaceBattle

    Private Enum BattleResult
        Win
        Loss
        Draw
    End Enum

    Dim Age As Integer
    Dim Location As Integer
    Dim WaitForInput As Func(Of String)

    Dim AlliesDamageDealt(3) As Integer
    Dim EnemyDamageDealt(3) As Integer

    Private RemainingCombatants As List(Of Ship)

    Private ReadOnly Property Allies As List(Of Ship)
        Get
            Return RemainingCombatants.Where(Function(s) (s.Owner.Team = Team.Human)).ToList()
        End Get
    End Property

    Private ReadOnly Property Hostiles As List(Of Ship)
        Get
            Return RemainingCombatants.Where(Function(s) (s.Owner.Team = Team.Enemy)).ToList()
        End Get
    End Property

    Public Sub New(Location As Integer, ReadLineDelegate As Func(Of String))
        Age = 0
        Me.Location = Location
        Me.WaitForInput = ReadLineDelegate

    End Sub

    Private Sub Intro(PlayerTeam As IEnumerable(Of Ship), Hostiles As IEnumerable(Of Ship))

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write("RED ALERT")
        ResetConsole()
        Console.WriteLine(" --------")

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("  Units entering combat!")
        ResetConsole()

        TabbedTeamList(PlayerTeam, "Allied units")
        TabbedTeamList(Hostiles, "Hostile forces")

        Console.WriteLine()
        Console.WriteLine("Press return")
        WaitForInput()
        Console.Clear()

    End Sub

    Private Sub Banner(PlayerTeam As IEnumerable(Of Ship), Hostiles As IEnumerable(Of Ship))

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write("BATTLE")
        ResetConsole()
        Console.WriteLine(" --------")

        TabbedTeamList(PlayerTeam, "Allied units")
        TabbedTeamList(Hostiles, "Hostile forces")

    End Sub

    Private Function FiringOrder(Combatants As List(Of Ship)) As List(Of Guid)
        Return Combatants _
            .OrderBy(Function(x) (x.Warp)) _
            .Select(Function(x) (x.Guid)) _
            .ToList()
    End Function

    Public Sub Fight(ByRef AllShips As List(Of ShipOrgUnit))

        'All things on this spot, with engaged flag set
        RemainingCombatants = AllShips _
                        .Where(Function(sh) (sh.Engaged AndAlso sh.Location = Me.Location)) _
                        .SelectMany(Function(sh) (sh.Ships)) _
                        .ToList()

        If (RemainingCombatants.Count = 0) Then
            'No one is fighting!
            Return
        End If

        'Display intro and wait for key press
        Intro(Allies, Hostiles)

        Dim NextShipToFire As Integer = 0

        Do
            'Ordered list of GUIDs representing firing order
            ' refreshed each time to stay up-to-date with ship deaths
            Dim RemainingShipIds = FiringOrder(RemainingCombatants)

            'Get the ship that's meant to fire next
            Dim S As Ship = RemainingCombatants.Where(Function(x) (x.Guid = RemainingShipIds(NextShipToFire))).Single()
            Console.WriteLine(" >> Now firing: {0}: {1} ({2})", NextShipToFire, S.Name, S.Guid)

            Banner(Allies, Hostiles)

            'Pick an enemy to fire at
            Dim Enemies = EnemiesOf(S, RemainingCombatants)
            Dim MinPercentHP = Enemies.Min(Function(e) (e.PercentHP))
            Dim Target = Enemies.Where(Function(e) (e.PercentHP = MinPercentHP)).FirstOrDefault()

            'Fire!
            Dim TargetDestroyed As Boolean = S.FireOn(Target, AllShips)

            'Remove from battle if dead
            If TargetDestroyed Then
                RemainingCombatants.Remove(Target)
            End If

            Console.WriteLine()
            Console.WriteLine("Press return")
            WaitForInput()

            'This is like a manual For loop
            ' which lets us remove ships but carry on (to the next one to fire)
            NextShipToFire += 1
            NextShipToFire = NextShipToFire Mod RemainingShipIds.Count

        Loop Until Allies.Count = 0 Or Hostiles.Count = 0

        AnnounceResult(Allies, Hostiles)
        Disengage(RemainingCombatants)

        WaitForInput()

    End Sub

    Private Sub KillShip(Victim As Ship, AllShips As List(Of ShipOrgUnit), ByRef RemainingCombatants As List(Of Ship))

        Victim.Die(AllShips)
        RemainingCombatants.Remove(Victim)

    End Sub

    Private Sub AnnounceResult(PlayerTeam As IEnumerable(Of Ship), Hostiles As IEnumerable(Of Ship))

        Console.WriteLine()
        Console.WriteLine("Result:")
        Console.WriteLine()
        Console.Write("  ")

        InvertConsole()

        Dim WinAnnouncement As String = " MUTUAL DESTRUCTION "
        If PlayerTeam.Count > 0 Then
            WinAnnouncement = " VICTORY "
        ElseIf Hostiles.Count > 0 Then
            WinAnnouncement = " DEFEAT "
        End If

        Console.WriteLine(WinAnnouncement)

        ResetConsole()

    End Sub

    Private Sub Disengage(ByRef Ships As IEnumerable(Of Ship))

        For Each S As Ship In Ships
            S.OrgUnit.Engaged = False
        Next

    End Sub

    Public Function EnemiesOf(ThisShip As Ship, Ships As IEnumerable(Of Ship)) As IEnumerable(Of Ship)

        Return Ships.Where(Function(AShip) (AShip.Owner.Team <> ThisShip.Owner.Team))

    End Function

End Class
