Public Class PlanetLanding

    Dim Planet As Planet

    Dim Lander As MobileEntity

    Public Sub New(Planet As Planet, Lander As MobileEntity)
        Me.Planet = Planet

        'N.b. Attacking player can be obtained with Lander.Owner
        Me.Lander = Lander

    End Sub

    Public Sub Land()

        Intro()

        Dim Succeeded As Boolean = False

        If Planet.Population = 0 Then
            Succeeded = Conquer(True)
        Else
            'TODO Conquest
            Succeeded = Conquer(False)
        End If

        If Succeeded Then
            Planet.Claim(Lander.Owner)
        End If

    End Sub

    Private Function Conquer(Uninhabited As Boolean) As Boolean

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

        Return True

    End Function

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

        Dim PlayerTeam = Lander.ShipContent
        TabbedTeamList(PlayerTeam, "Allied units")

        Console.WriteLine()
        Console.WriteLine("Press return")
        Console.ReadLine()
        Console.Clear()

    End Sub


End Class
