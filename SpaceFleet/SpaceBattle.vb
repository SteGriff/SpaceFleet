Public Class SpaceBattle

    Dim Age As Integer

    Public Sub New()
        Age = 0
    End Sub

    Private Sub Intro(PlayerTeam As List(Of Ship), Hostiles As List(Of Ship))

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write("RED ALERT")
        ResetConsole()
        Console.Write(" --------")

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("Units entering combat")
        ResetConsole()

        TabbedTeamList(PlayerTeam, "Allied units")
        TabbedTeamList(Hostiles, "Hostile forces")

        Console.WriteLine()
        Console.WriteLine("Press return")
        Console.ReadLine()
        Console.Clear()

    End Sub

    Private Sub Banner(PlayerTeam As List(Of Ship), Hostiles As List(Of Ship))

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Red
        Console.Write("BATTLE")
        ResetConsole()
        Console.Write(" --------")

        TabbedTeamList(PlayerTeam, "Allied units")
        TabbedTeamList(Hostiles, "Hostile forces")



    End Sub

    Private Sub TabbedTeamList(Team As List(Of Ship), TeamLabel As String)

        Console.WriteLine()
        Console.WriteLine("{0}:", TeamLabel)

        For Each S As Ship In Team
            Console.Write(vbTab)

            'Name in colour
            S.WriteName()

            'Hit points after name
            Console.WriteLine(" ({0}/{1}HP)", S.HP, S.MaxHP)
        Next

    End Sub

    Public Sub Fight(ByRef Ships As List(Of Ship))

        Dim PlayerTeam As List(Of Ship) = Ships.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
        Dim Hostiles As List(Of Ship) = Ships.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

        'Display intro and wait for key press
        Intro(PlayerTeam, Hostiles)

        Do
            'We'll check condition at end of loop (after firing)

            Banner(PlayerTeam, Hostiles)

            For Each S As Ship In Ships.OrderBy(Function(x) (x.Warp))

                Dim Enemies = EnemiesOf(S, Ships)
                Dim MinPercentHP = Enemies.Min(Function(e) (e.PercentHP))
                Dim Target = Enemies.Where(Function(e) (e.PercentHP = MinPercentHP)).FirstOrDefault()

                S.FireOn(Target)

                If S.Dead Then
                    Ships.Remove(S)
                End If

                If Target.Dead Then
                    Ships.Remove(Target)
                End If

            Next

            PlayerTeam = Ships.Where(Function(s) (TypeOf s.Owner Is Human)).ToList()
            Hostiles = Ships.Where(Function(s) (TypeOf s.Owner Is Enemy)).ToList()

            Console.WriteLine()
            Console.WriteLine("Press return")
            Console.ReadLine()

        Loop Until PlayerTeam.Count = 0 OrElse Hostiles.Count = 0

        Console.WriteLine("End of battle")
        CleanUp(Ships)

        Console.ReadLine()

    End Sub

    Private Sub CleanUp(ByRef Ships As List(Of Ship))

        For Each S As Ship In Ships
            S.Engaged = False
        Next

    End Sub

    Public Function EnemiesOf(ThisShip As Ship, Ships As List(Of Ship)) As List(Of Ship)

        Return Ships.Where(Function(AShip) (AShip.Owner.Race.Name <> ThisShip.Owner.Race.Name)).ToList()

    End Function

End Class
