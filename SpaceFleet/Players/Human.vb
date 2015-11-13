Public Class Human
    Inherits Player

    Public Sub New(R As Race, HomeStar As Star, InitialPlanets As List(Of Planet), UniversalShips As List(Of ShipOrgUnit))

        MyBase.New(0, R, HomeStar, InitialPlanets, UniversalShips)

    End Sub

    Public Overrides Sub Claim(Planet As Planet)

        Dim myPlanet = New MyPlanet(Planet)
        Me.Planets.Add(myPlanet)

    End Sub


End Class
