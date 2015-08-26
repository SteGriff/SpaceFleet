Public Class SpaceBattle

    Private Enum BattleResult
        Win
        Loss
        Draw
    End Enum

    Dim Age As Integer
    Dim Location As Integer

    Public Sub New(Location As Integer)
        Age = 0
        Me.Location = Location
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
        Console.ReadLine()
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

    Public Function AllDead(Team As List(Of Ship))

        Return Team.All(Function(s) (s.NoHealth))

    End Function

    Public Sub Fight(ByRef AllShips As List(Of MobileEntity))

        'All things on this spot, with engaged flag set
        Dim Combatants = AllShips _
                                  .Where(Function(sh) (sh.Engaged AndAlso sh.Location = Me.Location)) _
                                  .SelectMany(Function(sh) (sh.ShipContent)) _
                                  .ToList()

        Dim PlayerTeam As List(Of Ship) = Combatants.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
        Dim Hostiles As List(Of Ship) = Combatants.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

        'Dim Victory As BattleResult = BattleResult.Draw

        'Display intro and wait for key press
        Intro(PlayerTeam, Hostiles)

        Do
            For Each S As Ship In Combatants.OrderBy(Function(x) (x.Warp))

                Banner(PlayerTeam, Hostiles)

                If S.NoHealth Then
                    Exit For
                End If

                Dim Enemies = EnemiesOf(S, Combatants)
                Dim MinPercentHP = Enemies.Min(Function(e) (e.PercentHP))
                Dim Target = Enemies.Where(Function(e) (e.PercentHP = MinPercentHP)).FirstOrDefault()

                S.FireOn(Target)

                'Remove anyone dead from global ship register
                If S.NoHealth Then
                    S.Die(AllShips, Combatants)
                End If

                If Target.NoHealth Then
                    Target.Die(AllShips, Combatants)
                End If

                Console.WriteLine()
                Console.WriteLine("Press return")
                Console.ReadLine()

            Next

            PlayerTeam = Combatants.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
            Hostiles = Combatants.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

        Loop Until PlayerTeam.Count = 0 OrElse Hostiles.Count = 0

        AnnounceResult(PlayerTeam, Hostiles)
        Disengage(Combatants)

        Console.ReadLine()

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
            S.Engaged = False
        Next

    End Sub

    Public Function EnemiesOf(ThisShip As Ship, Ships As IEnumerable(Of Ship)) As IEnumerable(Of Ship)

        Return Ships.Where(Function(AShip) (AShip.Owner.Race.Name <> ThisShip.Owner.Race.Name))

    End Function

End Class
