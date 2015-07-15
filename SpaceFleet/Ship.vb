Public Class Ship
    Implements ICloneable, IConsoleEntity

    Public DesignName As String
    Public MyName As String
    Public HP As Byte
    Public MaxHP As Byte
    Public Warp As Byte
    Public MyLocation As Integer
    Public Destination As Integer

    Public Attack As Byte()
    Public Defence As Byte()

    Public Shared NullShip As New Ship()

    Public Const InfoTemplate As String = "{0,-22}{1,-6}{2,-12}{3,-12}{4,-8}"

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

    Sub New()
        Me.DesignName = ""
        Me.Name = ""
    End Sub

    Sub New(ByVal DesignName As String, ByVal HP As Byte, ByVal Warp As Byte, Attack As Byte(), Defence As Byte())

        Me.DesignName = DesignName
        Me.Name = DesignName & " " & Guid.NewGuid().ToString.Substring(0, 5)
        Me.HP = HP
        Me.MaxHP = HP
        Me.Warp = Warp

        Me.Attack = Attack
        Me.Defence = Defence

    End Sub

    Public ReadOnly Property Complexity As Decimal
        Get
            Return (2 * (Warp + MaxHP)) + (3 * (AttackTotal + DefenceTotal))
        End Get
    End Property

    Private ReadOnly Property AttackTotal As Integer
        Get
            Return Me.Attack(0) + Me.Attack(1) + Me.Attack(2)
        End Get
    End Property

    Private ReadOnly Property DefenceTotal As Integer
        Get
            Return Me.Defence(0) + Me.Defence(1) + Me.Defence(2)
        End Get
    End Property
    Public Sub Info()

        'Terran Defence Drone   2   100/100/100  100/100/100  999/999
        Console.WriteLine(InfoTemplate, Me.DesignName, Me.Warp, Me.AttackString, Me.DefenceString, Me.HpString)

    End Sub

    Public Function AttackString() As String
        Return Me.Attack(WeaponType.Laser) & "/" & Me.Attack(WeaponType.MassDriver) & "/" & Me.Attack(WeaponType.Missile)
    End Function

    Public Function DefenceString() As String
        Return Me.Defence(WeaponType.Laser) & "/" & Me.Defence(WeaponType.MassDriver) & "/" & Me.Defence(WeaponType.Missile)
    End Function

    Public Function HpString() As String
        Return HP & "/" & MaxHP
    End Function

    Public Function PercentBuilt(ProductionPoints As Decimal) As String
        Return Math.Round((ProductionPoints / Complexity) * 100, 1) & "%"
    End Function

    Function Clone() As Object Implements ICloneable.Clone

        Return New Ship(Me.DesignName, Me.HP, Me.Warp, Me.Attack, Me.Defence)

    End Function

    Function BuildClonedInstance(Location As Integer) As Ship

        Dim NewShip As Ship = Me.Clone()
        NewShip.Location = Location
        Return NewShip

    End Function

    Public Sub Draw() Implements IConsoleEntity.Draw

        Dim LocationString As String = ""
        If Moving() Then
            'TODO show number of weeks left in transport
            LocationString = String.Format("{0}pc -> {1}pc", Me.Location, Me.Destination)
        Else
            LocationString = String.Format("{0}pc", Me.Location)
        End If

        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.Black
        Console.WriteLine("{0}{1}{2} {3}pc", Me.Art(), vbTab, Me.Name, Me.Location)
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Gray

    End Sub

    Public Function Art() As String

        Dim Images() As String = {">", "}>", "}->", "}=>", "}]=>"}
        Dim Size As Integer = Math.Floor(MaxHP / 20)

        If Size > 4 Then
            Size = 4
        End If

        Return Images(Size)

    End Function

    Public Function Moving() As Boolean
        Return Location <> Destination
    End Function

    Public Sub Move()

        If Location < Destination Then
            Dim NewLocation As Integer = Location + Warp

            'Correct for overshoot
            If NewLocation > Destination Then
                NewLocation = Destination
            End If

            'Set location
            Location = NewLocation

        ElseIf Location > Destination Then
            Dim NewLocation As Integer = Location - Warp

            'Correct for overshoot
            If NewLocation < Destination Then
                NewLocation = Destination
            End If

            Location = NewLocation

        End If

    End Sub

End Class
