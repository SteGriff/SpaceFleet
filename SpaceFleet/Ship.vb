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

    Public Engaged As Boolean
    Public Owner As Player

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

    Public ReadOnly Property PercentHP As Decimal
        Get
            Return (HP / MaxHP) * 100
        End Get
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
        Console.WriteLine(InfoTemplate, Me.Name, Me.Warp, Me.AttackString, Me.DefenceString, Me.HpString)

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

    Public ReadOnly Property PercentBuilt(ProductionPoints As Decimal) As String
        Get
            Return Math.Round((ProductionPoints / Complexity) * 100, 1) & "%"
        End Get
    End Property

    Public ReadOnly Property Dead() As Boolean
        Get
            Return HP <= 0
        End Get
    End Property

    Function Clone() As Object Implements ICloneable.Clone

        Return New Ship(Me.DesignName, Me.HP, Me.Warp, Me.Attack, Me.Defence)

    End Function

    Function BuildClonedInstance(Owner As Player) As Ship

        Dim NewShip As Ship = Me.Clone()
        NewShip.Owner = Owner
        NewShip.Location = Owner.ConstructionPlanet.Location
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

    Public Sub Move(OtherShips As List(Of Ship))

        Dim OldLocation As Integer = Location

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

        'Check if we passed another ship
        For Each S As Ship In OtherShips
            If S.IsBetween(OldLocation, Location) Then
                Me.Location = S.Location
                Me.Engaged = True
                S.Engaged = True
            End If
        Next

    End Sub

    Public Function IsBetween(A As Integer, B As Integer)

        If A > B Then
            Return A > Me.Location AndAlso Me.Location > B
        ElseIf A < B Then
            Return A < Me.Location AndAlso Me.Location < B
        Else
            'A == B
            Return A = Me.Location
        End If

    End Function

    Public Sub FireOn(Target As Ship)

        Console.WriteLine("{0} firing on {1}...", Me.Name, Target.Name)

        Dim Damages(3) As Integer
        Dim BestDamage As Integer = 0
        Dim BestWeapon As WeaponType = WeaponType.Laser

        For W As Integer = 0 To 2
            Dim ThisDamage As Integer = Me.Attack(W) - Target.Defence(W)
            Damages(W) = ThisDamage

            Console.WriteLine("  {0} would do {1} damage", CType(W, WeaponType).ToString(), ThisDamage)

            If ThisDamage > BestDamage Then
                BestDamage = ThisDamage
                BestWeapon = CType(W, WeaponType)
                Console.WriteLine("  {0} is best weapon", BestWeapon.ToString())
            End If
        Next

        Target.HP -= BestDamage
        Console.WriteLine("{0} hit {1} with {2} for {3} points of damage", Me.Name, Target.Name, BestWeapon.ToString(), BestDamage)

        If (Target.HP <= 0) Then
            Console.WriteLine("{0} was destroyed!!", Target.Name)
        End If

    End Sub

End Class
