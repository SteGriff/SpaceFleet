Imports SpaceFleet

Module Mocks

    Public Function GetTestPlayer(AllUnits As List(Of ShipOrgUnit)) As Player

        Dim SomeRace As New Race("Testers", "._.", ConsoleColor.Blue)
        Dim HomeStar As New Star(0, True)
        Dim FirstPlanets As New List(Of Planet)({New Planet("Home", 0, 1, 2, 1, ConsoleColor.Blue, "O")})

        Return New Player(1, SomeRace, HomeStar, FirstPlanets, AllUnits)

    End Function

    Public Function GetTestEnemyPlayer(AllUnits As List(Of ShipOrgUnit)) As Player

        Dim SomeRace As New Race("Badguys", ">_<", ConsoleColor.Red)
        Dim HomeStar As New Star(100, True)
        Dim FirstPlanets As New List(Of Planet)({New Planet("Badland", 99, 1, 2, 1, ConsoleColor.Red, "(X)")})

        Return New Player(2, SomeRace, HomeStar, FirstPlanets, AllUnits)

    End Function

    Public Function GetTestShip() As Ship

        Return New Ship("Fighter", 1, 1, New Byte() {1, 1, 1}, New Byte() {1, 1, 1})

    End Function

End Module
