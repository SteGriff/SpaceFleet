Public Class Fleet
    Inherits MovingConsoleEntity

    Public ReadOnly Property Art As String
        Get
            Return Me.Size & "»"
        End Get
    End Property

    Public Sub New(Owner As Player, Location As Integer, AllShips As List(Of Ship))

        MyBase.New()

        Ships = AllShips.Where(Function(s) (s.Location = Location AndAlso Not s.Moving AndAlso s.Owner.Equals(Owner)))
        Me.Owner = Owner

        FleetNumber = Me.Owner.TopFleetNumber + 1
        MyName = String.Format("{0} fleet {1}", Owner.Race.Name, FleetNumber)

    End Sub

End Class
