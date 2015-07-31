Public Class Human
    Inherits Player

    Public Sub New(R As Race, InitialPlanets As List(Of Planet))

        MyBase.New(R)

        'Initialise friendly planets
        For Each P In InitialPlanets
            P.Claim(Me)
        Next

        ConstructionPlanet = Me.Planets(0)

    End Sub

End Class
