Public Class PlanetLanding

    Dim Planet As Planet

    Dim Ship As Ship

    Public Sub New(Planet As Planet, Ship As Ship)
        Me.Planet = Planet

        'N.b. Attacking player can be obtained with Ship.Owner
        Me.Ship = Ship

    End Sub

    Public Sub Land()

        Intro()

        If Planet.Population = 0 Then
            Conquer(True)
        Else
            'TODO Conquest

        End If

    End Sub

    Private Sub Conquer(Uninhabited As Boolean)

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write("LANDING ")
        Console.ForegroundColor = ConsoleColor.Green
        Console.Write("[SUCCESS]")
        ResetConsole()
        Console.WriteLine(" --------")

        Console.WriteLine()

        If Uninhabited Then
            Console.Write("Uninhabited planet, ")
            Planet.WriteName()
            Console.WriteLine(", was subdued!")
        Else
            Planet.Owner.Race.WriteName()
            Console.Write(" planet, ")
            Planet.WriteName()
            Console.WriteLine(", was overthrown!")
        End If

        ResetConsole()

    End Sub

    Private Sub Intro()

        Console.Clear()
        Console.WriteLine()

        Console.Write("--------- ")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write("LANDING")
        ResetConsole()
        Console.WriteLine(" --------")

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Blue
        Console.WriteLine("  Unit attempting foreign planet landing")
        ResetConsole()

        Dim PlayerTeam As New List(Of Ship)({Ship})
        TabbedTeamList(PlayerTeam, "Allied units")

        Console.WriteLine()
        Console.WriteLine("Press return")
        Console.ReadLine()
        Console.Clear()

    End Sub


End Class
