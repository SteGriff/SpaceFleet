Public Class Player
    Implements IPlayer

    Public Race As Race

    Public Money As Integer = 1000

    Public ProductionPoints As Decimal

    'Ship and Ship Design init
    Public Ships As New List(Of Ship)
    Public ShipDesigns As New List(Of Ship)
    Public CurrentlyBuilding As Ship

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

    Public Sub New(R As Race)

        Me.Race = R
        InitialiseShips()

    End Sub

    Public Sub InitialiseShips()

        Dim Attack As Byte() = {1, 0, 0}
        Dim Defence As Byte() = {1, 1, 1}

        ShipDesigns.Add(New Ship(Race.Name + " defence drone", 2, 2, Attack, Defence))
        ShipDesigns.Add(New Ship(Race.Name + " frigate", 2, 2, Attack, Defence))

        CurrentlyBuilding = ShipDesigns(0)
        ProductionPoints = 0

        Ships.Add(CType(ShipDesigns(0).BuildClonedInstance(0), Ship))
        Ships.Add(CType(ShipDesigns(1).BuildClonedInstance(100), Ship))

    End Sub
End Class
