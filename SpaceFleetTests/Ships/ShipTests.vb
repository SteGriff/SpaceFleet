Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SpaceFleet

Namespace SpaceFleet.Tests

    <TestClass()>
    Public Class ShipTests

        <TestMethod()>
        Public Sub MyNewShipIsOnMyTeam()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up human
            Dim Human = GetTestPlayer(AllUnits)

            'Destroy initial ship
            Human.ShipOrgs(0).Ships(0).Die(AllUnits)

            'Design a fast ship and assign it
            Dim FastDesign As Ship = New Ship("Fast fighter", 5, 100, New Integer() {1, 1, 1}, New Integer() {1, 1, 1})
            Dim NewOrg As New ShipOrgUnit(Human, FastDesign, AllUnits)

            'Check the owner and team for the org
            Assert.IsTrue(NewOrg.Owner.Equals(Human))
            Assert.IsTrue(NewOrg.Owner.Team = Team.Human)

            'Get the ship
            Dim MyNewShip = Human.ShipOrgs.Single().Ships.Single()

            'Check the owner and team for the ship
            Assert.IsTrue(MyNewShip.Owner.Equals(Human))
            Assert.IsTrue(MyNewShip.Owner.Team = Team.Human)

        End Sub
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

        <TestMethod()> Public Sub FireOnTest()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up two players, each will have one ship
            Dim Human = GetTestPlayer(AllUnits)
            Dim Enemy = GetTestEnemyPlayer(AllUnits)

            Dim EnemyShip = Enemy.ShipOrgs.Single().Ships.Single()
            Dim HumanShip = Human.ShipOrgs.Single().Ships.Single()

            'Human fires on Enemy
            HumanShip.FireOn(EnemyShip, AllUnits)

            'Test that it loses some health
            Assert.IsTrue(EnemyShip.HP < EnemyShip.MaxHP)

        End Sub

        <TestMethod()> Public Sub ShipDiesIfFiredOnRepeatedly()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up two players, each will have one ship
            Dim Human = GetTestPlayer(AllUnits)
            Dim Enemy = GetTestEnemyPlayer(AllUnits)

            Dim EnemyShip = Enemy.ShipOrgs.Single().Ships.Single()
            Dim HumanShip = Human.ShipOrgs.Single().Ships.Single()

            'Fire until the enemy has 0 HP
            Do Until EnemyShip.NoHealth

                HumanShip.FireOn(EnemyShip, AllUnits)

                'Test that it loses some health
                Assert.IsTrue(EnemyShip.HP < EnemyShip.MaxHP)

            Loop

            'Enemy ship is dead
            Assert.IsTrue(EnemyShip.NoHealth)

            'Enemy has no ships
            Assert.IsTrue(Enemy.ShipOrgs.Count = 0)

        End Sub
    End Class


End Namespace


