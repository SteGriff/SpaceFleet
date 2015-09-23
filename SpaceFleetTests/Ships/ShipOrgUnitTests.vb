Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports SpaceFleet


Namespace SpaceFleet.Tests

    <TestClass()> Public Class ShipOrgUnitTests

        ''' <summary>
        ''' Test that when a New Player is created, it comes with a ShipOrgUnit
        ''' and that that ShipOrgUnit is added to the global register
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()>
        Public Sub ShipOrgInitTest()

            Dim AllUnits As New List(Of ShipOrgUnit)()
            Dim ThePlayer = GetTestPlayer(AllUnits)

            'Test that a new 
            Assert.IsTrue(AllUnits.Count = 1)
            Assert.IsTrue(AllUnits.Contains(ThePlayer.ShipOrgs.Single()))

        End Sub

        <TestMethod()>
        Public Sub MergeFleetTest()

            Dim AllUnits As New List(Of ShipOrgUnit)()

            Dim ThePlayer = GetTestPlayer(AllUnits)
            Dim BasicShip = GetTestShip()

            Dim FirstUnit = ThePlayer.ShipOrgs(0)
            Dim SecondUnit As New ShipOrgUnit(ThePlayer, BasicShip, AllUnits)

            'Set up so that SecondUnit moves to location of FirstUnit
            ' and is absorbed into the fleet of FirstUnit
            FirstUnit.Location = 1
            FirstUnit.Destination = 1

            SecondUnit.Location = 0
            SecondUnit.Destination = 1

            SecondUnit.Move(AllUnits)

            'Test that:
            'Units are in correct places
            Assert.IsTrue(FirstUnit.Location = 1)
            Assert.IsTrue(SecondUnit.Location = 1)

            'First unit has absorbed second unit
            Assert.IsTrue(FirstUnit.IsFleet)

            'Only one ShipOrg in global register
            Assert.IsTrue(AllUnits.Count = 1)

        End Sub

        <TestMethod()>
        Public Sub DisbandFleetTest()

            Dim AllUnits As New List(Of ShipOrgUnit)()

            Dim ThePlayer = GetTestPlayer(AllUnits)
            Dim BasicShip = GetTestShip()

            'Create unit with 2 ships to form a fleet
            Dim FleetUnit = ThePlayer.ShipOrgs(0)
            FleetUnit.Ships.Add(BasicShip)

            FleetUnit.Location = 2
            FleetUnit.Destination = 2

            'Check that the player has 1 unit of 2 ships before we start tests
            Assert.IsTrue(ThePlayer.ShipOrgs.Count = 1)
            Assert.IsTrue(ThePlayer.ShipOrgs.Single().Size = 2)

            'The method to test:
            ' Disband the unit
            FleetUnit.DisbandFleet(AllUnits)

            'Test that:
            'Player has 2 units
            Assert.IsTrue(ThePlayer.ShipOrgs.Count = 2)

            'With 1 ship each
            Assert.IsTrue(ThePlayer.ShipOrgs.All(Function(org) (org.Size = 1)))

            'Units have not gone awry
            Assert.IsTrue(ThePlayer.ShipOrgs.All(Function(org) (org.Location = 2)))

            'Units are not going awry
            Assert.IsTrue(ThePlayer.ShipOrgs.All(Function(org) (org.Destination = 2)))

            '2 ShipOrgs in global register
            Assert.IsTrue(AllUnits.Count = 2)

        End Sub


    End Class


End Namespace


