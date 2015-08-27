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

    Public Overrides ReadOnly Property ShipContent As List(Of Ship)
        Get
            Return Me.Ships
        End Get
    End Property

    'Warp of the fleet = warp of the slowest ship
    Public Overrides ReadOnly Property Warp As Integer
        Get
            Return Ships.Min(Function(s) (s.Warp))
        End Get
    End Property

    Public Sub New(ByRef Owner As Player, Location As Integer, AllShips As List(Of MobileEntity))

        MyBase.New()

        Me.Owner = Owner
        FleetNumber = Me.Owner.TopFleetNumber + 1
        Owner.TopFleetNumber += 1

        MyName = String.Format("{0} fleet {1}", Owner.Race.Name, FleetNumber)

        AssembleFleet(Owner, Location, AllShips)

        SpaceFleet.ReportMessages.Add(String.Format("Formed: {0} ({1} ships)", MyName, Me.Ships.Count))

    End Sub

    Public Sub AssembleFleet(Owner As Player, Location As Integer, AllShips As List(Of MobileEntity))

        Debug.WriteLine("Fleet assembling for " & Owner.Name & " at " & Location)

        Me.Location = Location
        Me.Destination = Location

        Ships = New List(Of Ship)
        Dim Entities As New List(Of MobileEntity)

        'Get allied stuff stopped on this spot
        Dim CandidateEntities = AllShips.Where(Function(s) (s.Location = Location AndAlso _
                                            Not s.Moving AndAlso _
                                            s.Owner.Equals(Owner)))

        'Join and associate the ships to the fleet
        For Each S In CandidateEntities.Where(Function(x) (TypeOf x Is Ship))
            DirectCast(S, Ship).JoinFleet(Me)
        Next

        'Now get the fleets
        Dim FleetsToProcess = CandidateEntities.Where(Function(x) (TypeOf x Is Fleet)).ToList()

        'Break up any other fleets on this spot
        ' and move the ships into this fleet (all done in Fleet.MergeInto())
        Do While FleetsToProcess.Count > 0

            'Get next fleet
            Dim FE = FleetsToProcess.GetEnumerator()
            FE.MoveNext()
            Dim F = FE.Current

            'Merge it
            DirectCast(F, Fleet).MergeInto(Me, AllShips)

            'Don't process this fleet again
            FleetsToProcess.Remove(F)

        Loop

        'Remove the ships from ship register
        AllShips.RemoveAll(Function(s) (CandidateEntities.Contains(s)))

        'Add the fleet to ship register
        AllShips.Add(Me)

    End Sub

    Public Sub MergeInto(ByRef Fleet As Fleet, AllShips As List(Of MobileEntity))

        Debug.WriteLine("Fleet " + Me.Name + " merging into " + Fleet.Name)

        'Don't merge into self - it never ends!
        If Fleet.Name = Me.Name Then
            Debug.WriteLine("  Aborted because fleets are same")
            Return
        End If

        'Get every ship to leave the fleet and join the other
        Do While Me.Ships.Count > 0
            Dim SE = Me.Ships.GetEnumerator()
            SE.MoveNext
            SE.Current.LeaveFleet(AllShips)
            SE.Current.JoinFleet(Fleet)
        Loop

        'Remove the fleet from the register
        AllShips.Remove(Me)

        SpaceFleet.ReportMessages.Add(String.Format("Fleet {0} merged into {1} (now {2} ships)", _
                                                    Me.Name, Fleet.Name, Fleet.Ships.Count))

    End Sub

    Public Sub DisbandFleet(AllShips As List(Of MobileEntity))

        Debug.WriteLine("Fleet disbanding at " & Location)

        Do While Me.Ships.Count > 0

            'Get every ship to leave the fleet
            For Each S As Ship In Me.Ships
                S.LeaveFleet(AllShips)
                'WARN bad pattern again
                Exit For
            Next

        Loop

        'Remove this fleet from the register
        AllShips.Remove(Me)

    End Sub

End Class
