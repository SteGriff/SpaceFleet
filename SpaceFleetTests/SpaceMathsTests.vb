Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SpaceFleet

Namespace SpaceFleet.Tests

    <TestClass()>
    Public Class SpaceMathsTests

        <TestMethod()>
        Public Sub SumOfAllLowerIntegers_BaseCase()

            Dim input As Integer = 1
            Dim expected As Integer = 1
            Dim actual As Integer = SpaceMaths.SumOfAllLowerIntegers(input)

            Assert.AreEqual(expected, actual)

        End Sub

        <TestMethod()>
        Public Sub SumOfAllLowerIntegers_Recursive()

            Dim input As Integer = 4
            Dim expected As Integer = 10
            Dim actual As Integer = SpaceMaths.SumOfAllLowerIntegers(input)

            Assert.AreEqual(expected, actual)

        End Sub

    End Class
End Namespace
