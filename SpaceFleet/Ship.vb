Public Class Ship
    Inherits MobileEntity
    Implements ICloneable, IConsoleEntity, IColourful

    Public DesignName As String
    Public HP As Byte
    Public MaxHP As Byte
    Public MyLocation As Integer
    Public Fleet As Fleet

    Public Attack As Byte()
    Public Defence As Byte()

    Public Const InfoTemplate As String = "{0,-24}{1,-6}{2,-12}{3,-12}{4,-8}"

    Public ReadOnly Property PercentHP As Decimal
        Get
            Return (HP / MaxHP) * 100
        End Get
    End Property

    Private MyWarp As Integer
    Public Overrides ReadOnly Property Warp As Integer
        Get
            Return MyWarp
        End Get
    End Property

    Public Overrides ReadOnly Property ShipContent As List(Of Ship)
        Get
            Return New List(Of Ship)({Me})
        End Get
    End Property

    Sub New()
        MyBase.New()
        Me.DesignName = ""
    End Sub

    Sub New(ByVal DesignName As String, ByVal HP As Byte, ByVal Warp As Byte, Attack As Byte(), Defence As Byte())
        MyBase.New()

        Me.DesignName = DesignName
        Me.Name = DesignName & " " & Guid.NewGuid().ToString.Substring(0, 5)
        Me.HP = HP
        Me.MaxHP = HP
        MyWarp = Warp

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

        'Terran Drone   2   100/100/100  100/100/100  999/999
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

    Public ReadOnly Property NoHealth() As Boolean
        Get
            Return HP <= 0
        End Get
    End Property

    Function Clone() As Object Implements ICloneable.Clone

        Return New Ship(Me.DesignName, Me.HP, Me.Warp, Me.Attack, Me.Defence)

    End Function

    Function BuildClonedInstance(Owner As Player, AllShips As List(Of MobileEntity)) As Ship

        Dim NewShip As Ship = Me.Clone()
        NewShip.Owner = Owner
        NewShip.Location = Owner.ConstructionPlanet.Location

        AllShips.Add(NewShip)

        Return NewShip

    End Function

    Public Overrides ReadOnly Property Art As String
        Get

            Dim Images() As String = {">", "}>", "}->", "}=>", "}]=>"}
            Dim Size As Integer = Math.Floor(MaxHP / 20)

            If Size > 4 Then
                Size = 4
            End If

            Return Images(Size)

        End Get
    End Property

    Public Sub FireOn(Target As Ship)

        Console.WriteLine()
        Me.WriteName()
        Console.Write(" firing on ")
        Target.WriteName()
        Console.WriteLine("...")

        Dim Damages(3) As Integer
        Dim BestDamage As Integer = 0
        Dim BestWeapon As WeaponType = WeaponType.Laser

        For W As Integer = 0 To 2
            Dim ThisDamage As Integer = Me.Attack(W) - Target.Defence(W)
            Damages(W) = ThisDamage

            'Console.WriteLine("  {0} would do {1} damage", CType(W, WeaponType).ToString(), ThisDamage)

            If ThisDamage > BestDamage Then
                BestDamage = ThisDamage
                BestWeapon = CType(W, WeaponType)
                'Console.WriteLine("  {0} is best weapon", BestWeapon.ToString())
            End If
        Next

        Target.HP -= BestDamage

        Threading.Thread.Sleep(500)
        Console.WriteLine("  ...{0} dealt {1} damage!", BestWeapon.ToString(), BestDamage)

        If (Target.HP <= 0) Then
            Console.Write("  ...")
            Target.WriteName()
            Console.WriteLine(" was destroyed!!")
        End If

    End Sub

    Public Sub Die(AllShips As List(Of MobileEntity), Combatants As List(Of Ship))

        LeaveFleet(AllShips)
        AllShips.Remove(Me)
        Combatants.Remove(Me)

    End Sub

    Public Sub JoinFleet(Fleet As Fleet)

        Debug.WriteLine(Me.Name + " joins fleet: " + Fleet.Name)

        'Set fleet pointer
        Me.Fleet = Fleet
        Fleet.Ships.Add(Me)

    End Sub

    Public Sub LeaveFleet(AllShips As List(Of MobileEntity))

        'Leave fleet if possible
        If Not Me.Fleet Is Nothing Then

            'Remove self from fleet and unset my fleet pointer
            Me.Fleet.Ships.Remove(Me)
            Me.Fleet = Nothing

            'Add self to universal ship register
            AllShips.Add(Me)

        End If

    End Sub
End Class
