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

    'Which planet is used to send colonials to new conquests
    Public FeederPlanet As Planet

    'Fraction of people from feeder to send to new colonies
    Public SettlersPercent As Decimal

    Public TopFleetNumber As Integer

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

    Public ReadOnly Property Name As String Implements IPlayer.Name
        Get
            Return Me.Race.Name
        End Get
    End Property

    Public ReadOnly Property CanSpendMoney As Boolean
        Get
            Return Money > 1000
        End Get
    End Property

    Public Function HasInTerritory(S As MobileEntity) As Boolean

        Return TerritoryBegin <= S.Location AndAlso S.Location <= TerritoryEnd

    End Function

    Public Sub New(R As Race, HomeStar As Star, InitialPlanets As List(Of Planet), UniversalShips As List(Of MobileEntity))

        Me.Race = R

        TerritoryBegin = HomeStar.Location
        TerritoryEnd = HomeStar.Location

        For Each P As Planet In InitialPlanets
            P.Claim(Me)
        Next

        ConstructionPlanet = Me.Planets(0)
        InitialiseShips(UniversalShips)

    End Sub

    Public Sub InitialiseShips(UniversalShips As List(Of MobileEntity))

        Dim Attack As Byte() = {1, 0, 0}
        Dim Defence As Byte() = {0, 0, 0}

        ShipDesigns.Add(New Ship(Race.Name + " drone", 5, 2, Attack, Defence))
        'ShipDesigns.Add(New Ship(Race.Name + " frigate", 2, 2, Attack, Defence))

        CurrentlyBuilding = ShipDesigns(0)
        ProductionPoints = 0

        Dim FirstShip = CType(ShipDesigns(0).BuildClonedInstance(Me, UniversalShips), Ship)

        Ships.Add(FirstShip)

    End Sub

    Public Function TryBuildShip(UniversalShips As List(Of MobileEntity)) As Boolean

        'Production points have satisfied current build job
        If (ProductionPoints >= CurrentlyBuilding.Complexity) Then

            'Gain the ship by cloning the design into the ship roster
            Ships.Add(CType(CurrentlyBuilding.BuildClonedInstance(Me, UniversalShips), Ship))

            'Calculate leftover production pts
            ProductionPoints -= CurrentlyBuilding.Complexity
            Return True

        End If

        Return False

    End Function
End Class
