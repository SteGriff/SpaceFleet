Public Class EnemyGeneration

    Private Randomiser As Random

    Public Sub New(Randomiser As Random)
        Me.Randomiser = Randomiser
    End Sub

    Public Function GenerateEnemies(Stars As List(Of Star)) As List(Of Enemy)

        'TODO Allow races to spread across more than one star system initially

        Dim Enemies As New List(Of Enemy)

        For Each S As Star In Stars

            If S.Planets.Count = 0 Then
                'Nothing to claim in this system
                Continue For
            End If

            'Flip a coin and only populate star system for heads
            If Randomiser.Next(2) = 0 Then
                Continue For
            End If

            Dim ThisRace As Race = GenerateRace(S)

            'Instantiate the basic Enemy data (add it later)
            Dim ThisEnemy As New Enemy(ThisRace, Randomiser)

            'For later
            Dim SystemsControlled As Integer = Randomiser.Next(1, 4)
            Dim AnyPlanetInSystemClaimed As Boolean = False

            Dim TerritoryBegin As Integer = 0
            Dim TerritoryEnd As Integer = 0

            For Each P As Planet In S.Planets
                If S.Planets.Count = 1 Then
                    ClaimPlanet(ThisEnemy, P, TerritoryBegin, TerritoryEnd)
                    Exit For
                End If

                '1 in 2 chance to claim each planet in system
                If Randomiser.Next(2) = 0 Then
                    ClaimPlanet(ThisEnemy, P, TerritoryBegin, TerritoryEnd)
                End If

            Next

            'Claim at least one planet if none claimed
            If Not AnyPlanetInSystemClaimed Then
                ClaimPlanet(ThisEnemy, S.Planets(0), TerritoryBegin, TerritoryEnd)
            End If

            'Set the territory
            ThisEnemy.TerritoryBegin = TerritoryBegin
            ThisEnemy.TerritoryEnd = TerritoryEnd

            'Add the enemy to list
            Enemies.Add(ThisEnemy)

        Next

        Return Enemies

    End Function

    Public Sub ClaimPlanet(APlayer As IPlayer, ByRef P As Planet, ByRef TerritoryBegin As Integer, ByRef TerritoryEnd As Integer)

        'Set planet owner
        P.Owner = APlayer

        'Change race territory and pass out
        If TerritoryBegin = 0 Then
            TerritoryBegin = P.Location - APlayer.Influence
        End If

        If P.Location + APlayer.Influence > TerritoryEnd Then
            TerritoryEnd = P.Location + APlayer.Influence
        End If

    End Sub

    Private Function GenerateRace(S As Star)

        'Initialise face with eye template

        Dim Eyes() As String = {"^_^", "-_-", "+_+", "*_*", "**_**", ",_,", "`_`", "¬_¬", "@_@", "O_O", "o_o", "O_o", "o_O", "n_n", "u_u", "._.", ":_:", ";_;", ":._.:", "::_::", "x_x", "~_~"}
        Dim EyesNum As Integer = Randomiser.Next(Eyes.Length)

        Dim Face As String = Eyes(EyesNum)

        'Get random mouth
        Dim Mouths() As String = {"_", "__", "=", "==", ".", "..", "v", "u"}
        Dim MouthNum As Integer = Randomiser.Next(Mouths.Length)

        'Don't allow a mouth the same as the eye
        Do While Mouths(MouthNum).Contains(Face(0))
            MouthNum = Randomiser.Next(Mouths.Length)
        Loop

        'Replace mouth placeholder with a random mouthv
        Face = Face.Replace("_", Mouths(MouthNum))

        'Build head, replacing * with face content
        Dim HeadStructure() As String = {"(*)", "{*}", "[*]", "<*>", "\*/", "/*\", "|*|", "d*b"}

        Dim HeadStructureNum As Integer = Randomiser.Next(HeadStructure.Length)
        Dim Head As String = HeadStructure(HeadStructureNum)

        'Finalise head
        Head = Head.Replace("*", Face)

        'Get race name from star
        Dim NameFromStar = S.PotentialRaceNames(Randomiser.Next(S.PotentialRaceNames.Count))

        'Construct and return Race
        Dim NewRace As New Race(NameFromStar, Head, RandomConsoleColour(Randomiser))

        Return NewRace

    End Function

End Class
