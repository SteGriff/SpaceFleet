Public Class SpaceBattle

    Dim Ships As List(Of Ship)
    Dim Age As Integer

    Public Sub New(ByRef Combatants As List(Of Ship))
        Ships = Combatants
        Age = 0
    End Sub

    Public Sub Fight()

        'Dim PlayerTeam = Ships.Where(Function(s) (TypeOf s.Owner Is Player))
        'Dim Hostiles = Ships.Where(Function(s) (TypeOf s.Owner Is Enemy))

        For Each S As Ship In Ships.OrderBy(Function(x) (x.Warp))

            Dim Enemies = EnemiesOf(S)
            Dim MinPercentHP = Enemies.Min(Function(e) (e.PercentHP))
            Dim Target = Enemies.Where(Function(e) (e.PercentHP = MinPercentHP)).FirstOrDefault()

            S.FireOn(Target)

            If S.Dead Then
                Ships.Remove(S)
            End If

            If Target.Dead Then
                Ships.Remove(Target)
            End If

        Next

    End Sub

    Public Function EnemiesOf(ThisShip As Ship) As List(Of Ship)

        Return Ships.Where(Function(AShip) (AShip.Owner.Race.Name <> ThisShip.Owner.Race.Name))

    End Function

End Class
