Namespace SpaceMaths
    Public Module SpaceMaths

        Public Function SumOfAllLowerIntegers(anInt As Integer) As Integer
            Return If(anInt <= 1, anInt, anInt + SumOfAllLowerIntegers(anInt - 1))
        End Function

    End Module
End Namespace