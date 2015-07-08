Public Class PlanetGeneration

    Private Randomiser As Random

    Public Sub New(Randomiser As Random)
        Me.Randomiser = Randomiser
    End Sub

    Public Function GeneratePlanets() As Planet()

        Dim Planets As New List(Of Planet)
        Dim Stars As New List(Of Star)

        For s As Integer = 0 To 100

            Dim StarPlanetsHaveGreekLetters As Boolean = (Randomiser.Next(2) = 0)
            Dim ThisStar As New Star(GenerateStarName(), s, StarPlanetsHaveGreekLetters)
            Stars.Add(ThisStar)

            For p As Integer = 1 To Randomiser.Next(15)
                'Attempt to create 15 planets
                ' Some will be skipped to produce more interesting patterns

                If Randomiser.Next(3) = 0 Then
                    Planets.Add(GeneratePlanet(ThisStar, p))
                End If

            Next

        Next

        Return Planets.ToArray()

    End Function

    Public Function GeneratePlanet(Star As Star, Number As Integer) As Planet

        Dim Name As String = ""

        If Star.PlanetsHaveGreekLetters Then
            Name = Star.Name & "-" & GreekLetter(Number)
        Else
            Name = Star.Name & "-" & RomanNumeral(Number)
        End If

        'Size is temporary field used to calculate capacity, art, etc.
        ' 1 to 6
        Dim Size As Integer = 1 + Randomiser.Next(6)

        ' Use Art 0 to 5
        Dim Art As String = PlanetArt(Size)
        Dim Colour As ConsoleColor = RandomConsoleColour()

        Dim Capacity As Decimal = Size * (1 + Randomiser.NextDouble()) * 4
        Dim Population As Decimal = Capacity / (5 + Randomiser.Next(6))

        Dim Resources As Byte = Size * Randomiser.Next(10, 40)

        Dim Location As Long = Star.Location + (2 * Number)

        Dim Planet As New Planet(Name, Location, Population, Capacity, Resources, Colour, Art)

        Return Planet

    End Function

    Private Function PlanetArt(Size As Integer) As String

        Dim Arts() As String = {".", "°", "o", "O", "=O=", "(O)"}
        Return Arts(Size - 1)

    End Function

    Public Function RandomConsoleColour() As ConsoleColor

        'Don't use 0 - Black
        Dim Number As Integer = Randomiser.Next(1, 15)
        Return DirectCast(Number, ConsoleColor)

    End Function

    Public Function GenerateStarName() As String

        Dim Set1Starts() As String = {"Aer", "Bar", "Cret", "Don", "Eol", "Fler", "Gon", "Hyp", "Ion", "Iris", "Jac", "Kin", "Kar", "Lin", "Mor", "Nas", "Op", "Prel", "Pres", "Quas", "Quor", "Rin", "Sen", "Sys", "Syst", "Tin", "Tigas", "Trac", "Umbr", "Vac", "Vic", "Ward", "Xent", "Yenc", "Yint", "Zen", "Zentr"}
        Dim Set1Ends() As String = {"ion", "erion", "sen", "sec", "nac", "niz", "los", "lobe", "ten", "tera", "pio", "bio", "neo", "leo", "tora", "tres", "char", "ia", "io"}

        Dim Set2Starts() As String = {"Ace", "Aceta", "Belu", "Buta", "Cigo", "Copa", "Delo", "Doxo", "Etna", "Fogo", "Firba", "Gala", "Gilo", "Hopre", "Hapni", "Iono", "Irisa", "Jace", "Opa", "Pre", "Prese", "Tane", "Umbri", "Vico"}
        Dim Set2Ends() As String = {"bira", "bosal", "cera", "cino", "dina", "dul", "fena", "fira", "gion", "gira", "hera", "heron", "koen", "kina", "loom", "lino", "loma", "mina", "mira", "mino", "persi", "perim", "rese", "reon", "sente", "sion", "tira", "tisa", "vera", "viso", "vico", "xen", "xena", "zeta", "zera", "zal"}

        Dim Letters() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "A"}

        Select Case Randomiser.Next(9)
            Case 0 To 3
                Dim Part1 As String = Set1Starts(Randomiser.Next(Set1Starts.Length - 1))
                Dim Part2 As String = Set1Ends(Randomiser.Next(Set1Ends.Length - 1))

                Return Part1 & Part2

            Case 4 To 7
                Dim Part1 As String = Set2Starts(Randomiser.Next(Set2Starts.Length - 1))
                Dim Part2 As String = Set2Ends(Randomiser.Next(Set2Ends.Length - 1))

                Return Part1 & Part2

            Case Else
                'Only if = 8
                '2 to 4 random letters
                Dim Name As String = ""
                For i As Integer = 0 To 1 + Randomiser.Next(3)
                    Name += Letters(Randomiser.Next(26))
                Next
                Return Name

        End Select


    End Function

    Public Function RomanNumeral(Number As Integer) As String

        Dim Numerals() As String = {"I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV"}

        If Number > 0 And Number < 16 Then
            Return Numerals(Number - 1)
        Else
            Return "?"
        End If

    End Function

    Public Function GreekLetter(Number As Integer) As String

        Dim Letters() As String = {"Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Theta", "Iota", "Kappa", "Lambda", "Mu", "Nu", "Omicron", "Pi", "Psi", "Chi", "Phi", "Upsilon", "Omega"}
        Return Letters(Number - 1)

    End Function

End Class
