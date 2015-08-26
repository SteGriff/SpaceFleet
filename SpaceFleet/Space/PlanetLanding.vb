Public Class PlanetLanding

    Public Sub New()

    End Sub

    Public Sub Land(ByRef Planet As Planet, ByRef Lander As MobileEntity)

        'N.b. Attacking player can be obtained with Lander.Owner

        Intro(Lander)

        Dim Succeeded As Boolean = Conquer(Planet)

        If Succeeded Then
            Planet.Claim(Lander.Owner)
        End If

    End Sub

    Private Function Conquer(ByRef Planet As Planet) As Boolean

        Dim Uninhabited As Boolean = (Planet.Population = 0)

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

        'ResetConsole()

        PressReturnToClear()

        Return True

    End Function

    Private Sub Intro(Lander As MobileEntity)

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

        PressReturnToClear()

    End Sub

    Private Sub PressReturnToClear()
        Console.WriteLine()
        Console.WriteLine("Press return")
        Console.ReadLine()
        Console.Clear()
    End Sub
End Class
