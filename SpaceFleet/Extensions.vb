Public Module Extensions

    Function PopulationFormat(ByVal aPopulationValue) As Decimal
        Return Math.Round(aPopulationValue, 2)
    End Function

    Function ConvertNumberKey(ByVal NumericalKey As ConsoleKey) As Integer
        Return CInt(Right(NumericalKey.ToString, 1))
    End Function

    Public Function RandomConsoleColour(Randomiser As Random) As ConsoleColor

        'Don't use 0 - Black
        Dim Number As Integer = Randomiser.Next(1, 15)
        Return DirectCast(Number, ConsoleColor)

    End Function

    Public Sub TabbedTeamList(Team As IEnumerable(Of Ship), TeamLabel As String)

        Console.WriteLine()
        Console.WriteLine("{0}:", TeamLabel.ToUpper())

        For Each S In Team
            Console.Write("  ")

            'Name in colour
            S.WriteName()

            'Hit points after name
            Console.WriteLine(" ({0}/{1}HP)", S.HP, S.MaxHP)
        Next

    End Sub

    Public Sub ResetConsole()
        Console.ForegroundColor = ConsoleColor.Gray
        Console.BackgroundColor = ConsoleColor.Black
    End Sub

    Public Sub InvertConsole()
        Console.ForegroundColor = ConsoleColor.Black
        Console.BackgroundColor = ConsoleColor.Gray
    End Sub

End Module
