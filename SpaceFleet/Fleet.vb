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

    'Warp of the fleet = warp of the slowest ship
    Public Overrides ReadOnly Property Warp As Integer
        Get
            Return Ships.Min(Function(s) (s.Warp))
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

        Debug.WriteLine("Fleet assembling for " & Owner.Name & " at " & Location)

        Ships = New List(Of Ship)

        'Get allied ships stopped on this spot
        Dim CandidateShips = AllShips.Where(Function(s) (s.Location = Location AndAlso _
                                            Not s.Moving AndAlso _
                                            s.Owner.Equals(Owner)))

        'Join and associate the ships to the fleet
        For Each S As Ship In CandidateShips
            S.JoinFleet(Me)
        Next

        'Remove the ships from ship register
        AllShips.RemoveAll(Function(s) (CandidateShips.Contains(s)))

        'Add the fleet to ship register
        AllShips.Add(Me)

    End Sub

End Class
