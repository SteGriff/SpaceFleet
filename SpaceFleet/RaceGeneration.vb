Public Class RaceGeneration

    Private Randomiser As Random

    Public Sub New(Randomiser As Random)
        Me.Randomiser = Randomiser
    End Sub

    Public Function GenerateRaces(Stars As List(Of Star)) As List(Of Race)

        Dim Races As New List(Of Race)

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
            Races.Add(ThisRace)

            Dim SystemsControlled As Integer = Randomiser.Next(1, 4)
            Dim AnyPlanetInSystemClaimed As Boolean = False

            'TODO Claim random number of planets, make sure it claims at least one in system

            For Each P As Planet In S.Planets
                If S.Planets.Count = 1 Then
                    P.Owner = ThisRace
                    Exit For
                End If

                If Not AnyPlanetInSystemClaimed Then

                End If
            Next

        Next

        Return Races

    End Function

    Public Function GenerateRace(S As Star)

        'Dim NameStrategy As Integer = Randomiser.Next(0, 3)
        'Dim Name As String
        'Select Case NameStrategy
        '    Case 0

        'End Select


        'Initialise face with eye template

        Dim Eyes() As String = {"^_^", "-_-", "+_+", "*_*", "**_**", ",_,", "`_`", "¬_¬", "@_@", "O_O", "o_o", "O_o", "o_O", "n_n", "u_u", "._.", ">_<", ">_>", ":_:", ";_;", ":._.:", "::_::", "x_x"}
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
        Dim HeadStructure() As String = {"(*)", "{*}", "[*]", "<(*)>", "<*>", "<`*`>", "\*/", "/*\", "<\*/>", "'(*)'"}

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
