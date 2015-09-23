Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports SpaceFleet

Namespace SpaceFleet.Tests
    <TestClass()> Public Class SpaceBattleTests

        <TestMethod()> Public Sub FightTest()

            Dim AllUnits As New List(Of ShipOrgUnit)

            'Set up two players, each will have one ship
            Dim Human = GetTestPlayer(AllUnits)
            Dim Enemy = GetTestEnemyPlayer(AllUnits)

            'We need to give the human a faster ship, so we destroy its init one
            Human.ShipOrgs(0).Ships(0).Die(AllUnits)

            Dim FastDesign As Ship = New Ship("Fast fighter", 5, 100, New Byte() {1, 1, 1}, New Byte() {1, 1, 1})
            Dim NewOrg As New ShipOrgUnit(Human, FastDesign, AllUnits)

            'Put them in the same place
            Human.ShipOrgs(0).Location = 2
            Enemy.ShipOrgs(0).Location = 2

            'Start a fight there!
            Dim Battle As New SpaceBattle(2)

            Battle.Fight()

        End Sub
    End Class


End Namespace


