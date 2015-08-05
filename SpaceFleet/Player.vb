Public Class Player
    Implements IPlayer

    Public Race As Race

    Public Money As Integer = 1000

    Public ProductionPoints As Decimal

    'Techs
    Public Technologies(10) As Technology
    Public Researching As TechnologyType = TechnologyType.Research

    'Ship and Ship Design init
    Public Ships As New List(Of Ship)
    Public ShipDesigns As New List(Of Ship)
    Public CurrentlyBuilding As Ship

    'Which planet do ships come out of (by Planet index)
    Public ConstructionPlanet As Planet

    'Planets
    Private MyPlanets As New List(Of Planet)
    Public ReadOnly Property Planets As List(Of Planet)
        Get
            Return MyPlanets
        End Get
    End Property

    'Start and end of influenced territory
    Public TerritoryBegin As Integer
    Public TerritoryEnd As Integer

    'How far territory spreads from controlled planets
    Private MyInfluence As Integer
    Public Property Influence As Integer Implements IPlayer.Influence
        Get
            Return MyInfluence
        End Get
        Set(value As Integer)
            MyInfluence = value
        End Set
    End Property

    Public ReadOnly Property CanSpendMoney As Boolean
        Get
            Return Money > 1000
        End Get
    End Property

    Public Function HasInTerritory(S As Ship) As Boolean

        Return S.Location >= TerritoryBegin AndAlso S.Location <= TerritoryEnd

    End Function

    Public Sub New(R As Race, UniversalShips As List(Of Ship))

        Me.Race = R
        InitialiseShips(UniversalShips)

    End Sub

    Public Sub InitialiseShips(UniversalShips As List(Of Ship))

        Dim Attack As Byte() = {1, 0, 0}
        Dim Defence As Byte() = {0, 0, 0}

        ShipDesigns.Add(New Ship(Race.Name + " defence drone", 3, 2, Attack, Defence))
        'ShipDesigns.Add(New Ship(Race.Name + " frigate", 2, 2, Attack, Defence))

        CurrentlyBuilding = ShipDesigns(0)
        ProductionPoints = 0

        Dim FirstShip = CType(ShipDesigns(0).BuildClonedInstance(Me), Ship)

        Ships.Add(FirstShip)
        UniversalShips.Add(FirstShip)

    End Sub

    Public Function TryBuildShip() As Boolean

        'Production points have satisfied current build job
        If (ProductionPoints >= CurrentlyBuilding.Complexity) Then

            'Gain the ship by cloning the design into the ship roster
            Ships.Add(CType(CurrentlyBuilding.BuildClonedInstance(Me), Ship))

            'Calculate leftover production pts
            ProductionPoints -= CurrentlyBuilding.Complexity
            Return True

        End If

        Return False

    End Function
End Class
