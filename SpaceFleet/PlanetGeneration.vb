Public Class PlanetGeneration

    Private Randomiser As Random

    Public Sub New(Randomiser As Random)
        Me.Randomiser = Randomiser
    End Sub

    Public Function GeneratePlanets() As Planet()

        Dim Planets As New List(Of Planet)
        Dim Stars As New List(Of String)

        For s As Integer = 0 To 100

            Dim ThisStar As String = GenerateStarName()
            Stars.Add(ThisStar)

            For p As Integer = 0 To Randomiser.Next(15)
                'Attempt to create 15 planets
                ' Some will be skipped to produce more interesting patterns

                If Randomiser.Next(3) = 0 Then
                    Planets.Add(GeneratePlanet(ThisStar, p))
                End If

            Next

        Next

        Return Planets.ToArray()

    End Function

    Public Function GeneratePlanet(Star As String, Number As Integer) As Planet

        Dim Name As String = ""

        If Randomiser.Next(1) = 0 Then
            Name = Star & "-" & RomanNumeral(Number)
        Else
            Name = Star & "-" & GreekLetter(Number)
        End If

        'Size is temporary field used to calculate capacity, art, etc.
        ' 1 to 6
        Dim Size As Integer = 1 + Randomiser.Next(5)

        ' Use Art 0 to 5
        Dim Art As String = PlanetArt(Size)
        Dim Colour As ConsoleColor = RandomConsoleColour()

        Dim Capacity As Decimal = Size * (1 + Randomiser.NextDouble()) * 4
        Dim Population As Decimal = Capacity / (5 + Randomiser.Next(5))

        Dim Resources As Byte = Size * Randomiser.Next(10, 40)

        Dim Planet As New Planet(Name, Population, Capacity, Resources, Colour, Art)

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

        Dim Parts1() As String = {"Aer", "Bar", "Cret", "Don", "Eol", "Fler", "Gon", "Hyp", "Ion", "Jace", "Kin", "Kar", "Lin", "Mor", "Nas", "Opa", "Pre", "Prese", "Quas", "Quor", "Rin", "Sen", "Sys", "Tine", "Tigas", "Umb", "Umbri", "Vac", "Vico", "Ward", "Xen", "Yin", "Zen"}
        Dim Parts2() As String = {"ion", "erion", "sen", "sec", "nac", "niz", "los", "lobe", "ten", "tera", "pio", "bio", "neo", "leo", "tora", "tres", "char", "ia", "io"}

        Dim Letters() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}

        If Randomiser.Next(1) = 0 Then
            Dim Part1 As String = Parts1(Randomiser.Next(Parts1.Length - 1))
            Dim Part2 As String = Parts2(Randomiser.Next(Parts2.Length - 1))
            Dim Name As String = Part1 & Part2

            If Randomiser.Next(1) = 0 Then
                Dim Number As Integer = Randomiser.Next(1000) * 5
                Return Name & "-" & Number.ToString()
            End If

            Return Name
        Else
            Dim Name As String = ""
            For i As Integer = 0 To Randomiser.Next(3)
                Name += Letters(Randomiser.Next(25))
            Next
            Dim Number As Integer = Randomiser.Next(1000) * 5
            Return Name & "-" & Number.ToString()
        End If

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
