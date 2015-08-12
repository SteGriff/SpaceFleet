Public Class Fleet
    Inherits MobileEntity
    Implements IConsoleEntity, IColourful

    Public Ships As List(Of Ship)

    Public Overrides ReadOnly Property Art As String
        Get
            Return Me.Size & "»"
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Ships.Count
        End Get
    End Property

    Public Sub New(Owner As Player, Location As Integer, AllShips As List(Of MobileEntity))

        MyBase.New()

        Me.Owner = Owner
        FleetNumber = Me.Owner.TopFleetNumber + 1
        MyName = String.Format("{0} fleet {1}", Owner.Race.Name, FleetNumber)

        AssembleFleet(Owner, Location, AllShips)

    End Sub

    Public Sub AssembleFleet(Owner As Player, Location As Integer, AllShips As List(Of MobileEntity))

        Ships = AllShips.Where(Function(s) (s.Location = Location AndAlso _
                                            Not s.Moving AndAlso _
                                            s.Owner.Equals(Owner)))

    End Sub

End Class
