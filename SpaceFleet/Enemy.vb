Public Class Enemy
    Inherits Player

    'Difficulty
    Public Ability As Integer
    Public Age As Integer

    'Techs
    Public Technologies(10) As Technology
    Private Researching As TechnologyType = TechnologyType.Research

    'Territory core
    Dim HomeStar As Star

    'How many times have you met (called Meet())
    Private MyMeetings As Integer
    Public ReadOnly Property Meetings As Integer
        Get
            Return MyMeetings
        End Get
    End Property

    Public Sub New(R As Race, S As Star, Randomiser As Random)
        MyBase.New(R)

        Me.HomeStar = S

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

        'Build ships
        If Age Mod Math.Floor(100 / Ability) = 0 Then
            'Shortcut... build from home star for now
            Dim NewShip = ShipDesigns(0).BuildClonedInstance(HomeStar.Location)

            'Target the player's end of the lineiverse
            NewShip.Destination = 0

            Ships.Add(NewShip)
        End If

        'Move all ships
        For Each S As Ship In Ships
            S.Move()

        Next

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
