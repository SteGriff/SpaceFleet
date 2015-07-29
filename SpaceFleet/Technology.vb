Public Structure Technology

    Dim Field As TechnologyType
    Dim Level As Decimal
    Dim LevelledUpThisWeek As Boolean
    Dim Votes As Integer

    Sub New(ByVal Field As TechnologyType, ByVal Level As Decimal)

        Me.Field = Field
        Me.Level = Level

    End Sub

    Function Name()
        Return Me.Field.ToString
    End Function

    Function Improvement(ByVal TechIncome As Decimal, ByVal Technology() As Technology) As Decimal

        Dim Income As Decimal = (Math.Sqrt(TechIncome)) / 20
        Dim Ability As Decimal = (Math.Sqrt(Technology(TechnologyType.Research).Level))
        Dim Prospective As Decimal = Math.Round(Me.Level + (Income * Ability), 1)

        'Limit techs to max level 100
        Return Math.Min(100, Prospective)

    End Function

    Sub ImproveAndCheckAdvancement(ByVal TechIncome As Single, ByVal Technology() As Technology)

        Dim oldLevel As Single = Me.Level
        LevelledUpThisWeek = False

        Me.Level = Improvement(TechIncome, Technology)

        If Math.Floor(Me.Level) > Math.Floor(oldLevel) Then
            LevelledUpThisWeek = True
        End If

    End Sub
End Structure
