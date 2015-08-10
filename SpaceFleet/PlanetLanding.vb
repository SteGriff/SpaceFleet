Public Class PlanetLanding

    Dim Planet As Planet
    Dim Ship As Ship

    Public Sub New(Planet As Planet)
        Me.Planet = Planet
    End Sub

    Public Sub Land(Ship As Ship)

        Me.Ship  = Ship
        Intro()

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
