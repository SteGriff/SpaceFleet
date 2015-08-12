Public Class Human
    Inherits Player

    Public Sub New(R As Race, HomeStar As Star, InitialPlanets As List(Of Planet), UniversalShips As List(Of MobileEntity))

        MyBase.New(R, HomeStar, InitialPlanets, UniversalShips)

    End Sub

End Class
