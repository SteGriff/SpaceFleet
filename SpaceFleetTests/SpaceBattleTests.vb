Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SpaceFleet

Namespace SpaceFleet.Tests

    <TestClass()> Public Class SpaceBattleTests

        <TestMethod()> Public Sub ShipsPassingInTheNight()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up two players, each will have one ship
            Dim Human = GetTestPlayer(AllUnits)
            Dim Enemy = GetTestEnemyPlayer(AllUnits)

            'Put them in the same place
            Human.ShipOrgs(0).Location = 2
            Enemy.ShipOrgs(0).Location = 2

            'Precondition:
            ' 2 ships exist
            Assert.IsTrue(AllUnits.Count = 2)

            'Start a fight 
            'It should exit early, gracefully, with no fatalities
            Dim Battle As New SpaceBattle(2, AddressOf ReadLineDelegate)

            Battle.Fight(AllUnits)

            'Test that:
            'Nobody is dead
            Assert.IsTrue(AllUnits.Count = 2)

            'Still have one ship each 
            Assert.IsTrue(Human.ShipOrgs(0).Ships.Count = 1)
            Assert.IsTrue(Enemy.ShipOrgs(0).Ships.Count = 1)

        End Sub

        <TestMethod()> Public Sub FightTest()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up two players, each will have one ship
            Dim Human = GetTestPlayer(AllUnits)
            Dim Enemy = GetTestEnemyPlayer(AllUnits)

            'We need to give the human a faster ship, so we destroy its init one
            Human.ShipOrgs(0).Ships(0).Die(AllUnits)

            'Design the fast ship and assign it
            Dim FastDesign As Ship = New Ship("Fast fighter", 5, 100, New Integer() {1, 1, 1}, New Integer() {1, 1, 1})
            Dim NewOrg As New ShipOrgUnit(Human, FastDesign, AllUnits)

            'Put them in the same place
            ' and toggle their battle flags
            Human.ShipOrgs(0).Location = 2
            Human.ShipOrgs(0).Engaged = True

            Enemy.ShipOrgs(0).Location = 2
            Enemy.ShipOrgs(0).Engaged = True

            'Start a fight
            Dim Battle As New SpaceBattle(2, AddressOf ReadLineDelegate)
            Battle.Fight(AllUnits)

            'Test that:
            ' the enemy ship is dead
            Assert.IsTrue(Enemy.ShipOrgs.Count = 0)

            'My ship is not dead
            Assert.IsTrue(Human.ShipOrgs(0).Ships.Count = 1)
            Assert.IsTrue(Human.ShipOrgs(0).Ships(0).HP > 0)

        End Sub

        Public Function ReadLineDelegate() As String
            Return ""
        End Function

    End Class


End Namespace


