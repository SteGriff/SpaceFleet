Public Class SpaceBattle

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

    Public Sub Fight(ByRef AllShips As List(Of MobileEntity))

        'All things on this spot, with engaged flag set
        Dim PotentialCombatants = AllShips.Where(Function(sh) (sh.Engaged AndAlso sh.Location = Me.Location))
        Dim Combatants As New List(Of Ship)

        'Ehhh this could be done with LINQ
        For Each E As MobileEntity In PotentialCombatants
            If TypeOf E Is Ship Then
                Combatants.Add(E)
            Else
                Dim FleetShips = DirectCast(E, Fleet).Ships
                Combatants.AddRange(FleetShips)
            End If
        Next

        Dim PlayerTeam As List(Of Ship) = Combatants.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
        Dim Hostiles As List(Of Ship) = Combatants.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

        'Display intro and wait for key press
        Intro(PlayerTeam, Hostiles)

        Do
            For Each S As Ship In Combatants.OrderBy(Function(x) (x.Warp))

                Banner(PlayerTeam, Hostiles)

                If S.Dead Then
                    Exit For
                End If

                Dim Enemies = EnemiesOf(S, Combatants)
                Dim MinPercentHP = Enemies.Min(Function(e) (e.PercentHP))
                Dim Target = Enemies.Where(Function(e) (e.PercentHP = MinPercentHP)).FirstOrDefault()

                S.FireOn(Target)

                'Remove anyone dead from global ship register
                If S.Dead Then
                    AllShips.Remove(S)
                End If

                If Target.Dead Then
                    AllShips.Remove(Target)
                End If

                Console.WriteLine()
                Console.WriteLine("Press return")
                Console.ReadLine()

            Next

            PlayerTeam = Combatants.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
            Hostiles = Combatants.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

        Loop Until PlayerTeam.Count = 0 OrElse Hostiles.Count = 0

        AnnounceResult(PlayerTeam, Hostiles)
        CleanUp(Combatants)

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

    Private Sub CleanUp(ByRef Ships As IEnumerable(Of Ship))

        For Each S As Ship In Ships
            S.Engaged = False
        Next

    End Sub

    Public Function EnemiesOf(ThisShip As Ship, Ships As IEnumerable(Of Ship)) As IEnumerable(Of Ship)

        Return Ships.Where(Function(AShip) (AShip.Owner.Race.Name <> ThisShip.Owner.Race.Name))

    End Function

End Class
