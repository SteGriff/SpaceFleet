Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SpaceFleet

Namespace SpaceFleet.Tests

    <TestClass()>
    Public Class ShipTests

        <TestMethod()>
        Public Sub ShipDeathTest()

            'Fixtures
            Dim AllUnits As New List(Of ShipOrgUnit)()

            'Player should come with a ShipOrgUnit which is added to the register
            Dim aPlayer = GetTestPlayer(AllUnits)

            'Pre-test
            'AllUnits contains 1 thing
            Assert.IsTrue(AllUnits.Count = 1)

            'Kill the ship
            ' this line will handily fail if there are the wrong number of Orgs or Ships
            aPlayer.ShipOrgs.Single().Ships.Single().Die(AllUnits)

            'Test that:
            'AllUnits now contains 0 things
            Assert.IsTrue(AllUnits.Count = 0)

            'Player has no orgs
            Assert.IsTrue(aPlayer.ShipOrgs.Count = 0)

        End Sub
    End Class


End Namespace


