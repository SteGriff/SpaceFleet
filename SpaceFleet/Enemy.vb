Public Class Enemy
    Implements IPlayer

    Public Race As Race

    'Difficulty
    Public Ability As Integer
    Public Age As Integer

    'Techs
    Public Technologies(10) As Technology
    Private Researching As TechnologyType = TechnologyType.Research

    'Ship and Ship Design init
    Dim Ships As New List(Of Ship)
    Dim ShipDesigns As New List(Of Ship)

    'How far territory spreads from controlled planets
    Private MyInfluence As Integer
    Public Property Influence As Integer Implements IPlayer.Influence

    'Start and end of influenced territory
    Public TerritoryBegin As Integer
    Public TerritoryEnd As Integer

    'How many times have you met (called Meet())
    Private MyMeetings As Integer
    Public ReadOnly Property Meetings As Integer
        Get
            Return MyMeetings
        End Get
    End Property

    Public Sub New(R As Race, Randomiser As Random)
        Me.Race = R

        'Ability (difficulty) 1 to 10; 10 is most able
        Me.Ability = Randomiser.Next(1, 11)

        'Set all my techs to level 1
        InitialiseTechnologies(Technologies)

        Me.Influence = 1
        Me.MyMeetings = 0


        Dim Attack As Byte() = {1, 0, 0}
        Dim Defence As Byte() = {1, 1, 1}

        ShipDesigns.Add(New Ship(Race.Name + " defence drone", 2, 2, Attack, Defence))
        Ships.Add(CType(ShipDesigns(0).BuildClonedInstance(0), Ship))

    End Sub

    Public Function HasInTerritory(S As Ship) As Boolean

        Return S.Location >= TerritoryBegin AndAlso S.Location <= TerritoryEnd

    End Function

    Public Sub Meet()
        MyMeetings += 1
    End Sub

    Public Sub Turn()

        Age += 1

        'Tech
        'Fake tech income
        Dim TechIncome As Integer = (Ability / 2) + (Math.Sqrt(Age) / 10)

        'Do research
        Technologies(Researching).ImproveAndCheckAdvancement(TechIncome, Technologies)

        'Decide what to research next turn
        DecideResearch()



    End Sub

    Private Sub DecideResearch()

        'Get all techs with max votes
        Dim VotedTechs = Technologies.OrderByDescending(Function(x) (x.Votes))

        'If it contains research, pick that
        If VotedTechs.Contains(Technologies(TechnologyType.Research)) Then
            Researching = TechnologyType.Research
        Else
            'Pick one (based on arbitrary internal order)
            Researching = VotedTechs.First().Field
        End If

    End Sub

End Class
