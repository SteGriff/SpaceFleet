Public Module Extensions

    Function PopulationFormat(ByVal aPopulationValue) As Decimal
        Return Math.Round(aPopulationValue, 2)
    End Function

    Function ConvertNumberKey(ByVal NumericalKey As ConsoleKey) As Integer
        Return CInt(Right(NumericalKey.ToString, 1))
    End Function

End Module
