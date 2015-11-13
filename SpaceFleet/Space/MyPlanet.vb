Public Class MyPlanet
    Inherits Planet

    Private MyResearch As Byte
    Public Income As Byte

    Public ReadOnly Property Research As Decimal
        Get
            Return MyResearch * ResearchBonus
        End Get
    End Property

    Public ReadOnly Property Stability As Decimal
        Get
            'From 0.1 to infinity basically
            ' Less than 1 is bad
            Return Math.Min(10, CInt(Me.MyResearch) + CInt(Me.Income)) / 100
        End Get
    End Property

    Public ReadOnly Property FocusDescription As String
        Get
            If Focus = ProductionFocus.Balanced Then
                Return String.Format("{0} has balanced focus", Name)
            Else
                Return String.Format("{0} is focused on {1}.", Name, Focus.ToString.ToLower)
            End If
        End Get
    End Property

    Sub New(ByVal Name As String, Location As Long, ByVal Population As Decimal, ByVal Capacity As Decimal, ByVal TechnologyBenefit As Byte, ByVal MoneyBenefit As Byte, ByVal Resources As Byte, Art As String, Colour As ConsoleColor)

        MyBase.New(Name, Location, Population, Capacity, Resources, Colour, Art)

        Me.MyResearch = TechnologyBenefit
        Me.Income = MoneyBenefit
        Me.Focus = ProductionFocus.Balanced

    End Sub

    Sub New(Planet As Planet)
        MyBase.New(Planet.Name, Planet.Location, Planet.Population, Planet.Capacity, Planet.Resources, Planet.Colour, Planet.Art)

        Me.Focus = ProductionFocus.Balanced
        Me.MyResearch = 1
        Me.Income = 1

    End Sub


    Function GrowPopulation() As Decimal

        'Only if growth is needed
        If Me.Population < Me.Capacity Then

            'We want to fill faster if there's more free space (fast expansion)
            Dim FreeSpace As Integer = (Me.Capacity - Me.Population)
            Dim FreeSpacePercent As Integer = (FreeSpace / Me.Capacity) * 100

            'Based on a good measure of productivity:
            Dim Stability As Decimal = Me.Stability()

            'Fill a fraction of the free space, accounting for world development level.
            Dim PopulationIncrease As Decimal = Math.Round((FreeSpace / 6) * Stability, 2)

            'Fill the last few percent instantly to avoid 'approaching' max.
            If FreeSpacePercent <= 2 Then
                PopulationIncrease = FreeSpace
            End If

            'Boil it down to the new population figure in the correct data type
            Dim NewPopulation As Decimal

            'Make sure there's no logical problem with the new figure fitting in 256
            Try
                NewPopulation = Population + PopulationIncrease
            Catch ex As Exception
                NewPopulation = Me.Capacity
            End Try

            If NewPopulation > Capacity Then
                NewPopulation = Capacity
            End If

            'Checkline!
            'Console.WriteLine("{0} grew from {1} to {2} in maximum {3}.", Me.Name, Me.Population, NewPopulation, Me.Capacity)
            'Console.WriteLine("{0} ({1}): MaxGrowth {2}; Stability {3}; ActualGrowth {4} -> {5}/{6}", Me.Name, Me.Population, (FreeSpace / 2), Stability, PopulationIncrease, NewPopulation, Me.Capacity)

            'Set!
            Me.Population = NewPopulation

            Return PopulationIncrease

        End If

        Return 0

    End Function

    Sub GrowIncome()

        'Each member of the population uses the cumulative planet resources to make money this turn. Limit to 255.
        Dim NewIncome As Short = Math.Min(255, CShort(PopulationFormat(Me.Population / 2)) * CShort(Me.Resources))
        Me.Income = CByte(NewIncome)

    End Sub

    Sub GrowResearch()

        'A random amount of the planets resources is added to its current research ability. Limit to 255.
        Dim NewResearch As Short = Math.Min(255, Math.Max(Me.MyResearch, (Me.MyResearch - 1 + Int(Rnd() * Me.Resources))))
        Me.MyResearch = CByte(NewResearch)

    End Sub

End Class