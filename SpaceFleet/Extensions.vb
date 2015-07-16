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

End Module
