Public Class Enemy
    Inherits Player

    'Difficulty
    Public Ability As Integer
    Public Age As Integer

    'How many times have you met (called Meet())
    Private MyMeetings As Integer
    Public ReadOnly Property Meetings As Integer
        Get
            Return MyMeetings
        End Get
    End Property

    Public Sub New(R As Race, S As Star, UniversalShips As List(Of Ship), InitialPlanets As List(Of Planet), Randomiser As Random)
        MyBase.New(R, S, InitialPlanets, UniversalShips)

        'Ability (difficulty) 1 to 10; 10 is most able
        Me.Ability = Randomiser.Next(1, 11)

        'Set all my techs to level 1
        InitialiseTechnologies(Technologies)

        Me.Influence = 1
        Me.MyMeetings = 0

    End Sub

    Public Sub Meet()
        MyMeetings += 1
    End Sub

    Public Sub Turn(OtherShips As List(Of Ship))

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
            Dim NewShip = ShipDesigns(0).BuildClonedInstance(Me, OtherShips)

            'Target the player's end of the lineiverse
            NewShip.Destination = 0

            Ships.Add(NewShip)
        End If

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
