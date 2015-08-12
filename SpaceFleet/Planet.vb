Public Class Planet
    Implements IConsoleEntity, IColourful

    Public MyName As String
    Public Population As Decimal ' x billion people
    Public Capacity As Decimal
    Public MyLocation As Integer

    Public Resources As Byte

    Public Colour As ConsoleColor
    Public Art As String

    Public Owner As Player

    Public Focus As ProductionFocus

    ReadOnly Property ResearchBonus As Decimal
        Get
            If Focus = ProductionFocus.Balanced Then
                Return 1
            ElseIf Focus = ProductionFocus.Production Then
                Return 0.5
            ElseIf Focus = ProductionFocus.Research Then
                Return 1.5
            End If
        End Get
    End Property

    ReadOnly Property ProductionBonus As Decimal
        Get
            If Focus = ProductionFocus.Balanced Then
                Return 1
            ElseIf Focus = ProductionFocus.Production Then
                Return 1.5
            ElseIf Focus = ProductionFocus.Research Then
                Return 0.5
            End If
        End Get
    End Property

    Public ReadOnly Property Production As Decimal
        Get
            Return 0.8 * Resources * ProductionBonus
        End Get
    End Property

    Public Property Location As Integer Implements IConsoleEntity.Location
        Get
            Return MyLocation
        End Get
        Set(value As Integer)
            MyLocation = value
        End Set
    End Property

    Public Property Name As String Implements IConsoleEntity.Name
        Get
            Return MyName
        End Get
        Set(value As String)
            MyName = value
        End Set
    End Property

    Public Sub New(Name As String, Location As Integer, Population As Decimal, Capacity As Decimal, Resources As Byte, Colour As ConsoleColor, Art As String)
        Me.Name = Name
        Me.Location = Location
        Me.Population = Population
        Me.Capacity = Capacity
        Me.Resources = Resources
        Me.Colour = Colour
        Me.Art = Art
    End Sub

    Public Sub Draw() Implements IConsoleEntity.Draw

        Console.ForegroundColor = Me.Colour
        Console.Write(Me.Art)
        ResetConsole()
        Console.WriteLine(vbTab & "{0} {1}pc", Me.Name, Me.Location)

    End Sub

    Public Sub WriteName() Implements IColourful.WriteName

        Console.ForegroundColor = Me.Colour
        Console.Write(Me.Name)
        ResetConsole()

    End Sub

    Public Sub Claim(Claimant As Player)

        'Set planet owner
        Me.Owner = Claimant
        Claimant.Planets.Add(Me)

        If Me.Population = 0 Then
            Me.PopulateSparsely()
        End If

        'Change race territory and pass out
        If Me.Location - Claimant.Influence < Claimant.TerritoryBegin Then
            Claimant.TerritoryBegin = Me.Location - Claimant.Influence
        End If

        If Me.Location + Claimant.Influence > Claimant.TerritoryEnd Then
            Claimant.TerritoryEnd = Me.Location + Claimant.Influence
        End If

    End Sub

    Public Function ClaimableBy(Claimant As Player) As Boolean

        If Me.Owner Is Nothing Then
            Return True
        End If

        Return Me.Owner.Name <> Claimant.Name

    End Function

    Private Sub PopulateSparsely()
        Population = Capacity / (5 + Rnd(6))
    End Sub

End Class
