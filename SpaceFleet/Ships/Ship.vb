Public Class Ship
    Implements ICloneable, ITransponding

    Public Name As String
    Public DesignName As String
    Public Guid As Guid
    Public HP As Integer
    Public MaxHP As Integer
    Public OrgUnit As ShipOrgUnit

    Public Attack As Integer()
    Public Defence As Integer()

    Public Const InfoTemplate As String = "{0,-24}{1,-6}{2,-12}{3,-12}{4,-8}"

    Public ReadOnly Property Owner As Player
        Get
            Return OrgUnit.Owner
        End Get
    End Property

    Public ReadOnly Property PercentHP As Decimal
        Get
            Return (HP / MaxHP) * 100
        End Get
    End Property

    Private MyWarp As Integer
    Public ReadOnly Property Warp As Integer
        Get
            Return MyWarp
        End Get
    End Property

    Sub New()

        AssignId()
        Me.DesignName = ""

    End Sub

    Sub New(ByVal DesignName As String, ByVal HP As Byte, ByVal Warp As Byte, Attack As Integer(), Defence As Integer())

        AssignId()
        Me.DesignName = DesignName
        Me.Name = DesignName & " " & Me.Guid.ToString.Substring(0, 5)
        Me.HP = HP
        Me.MaxHP = HP
        MyWarp = Warp

        Me.Attack = Attack
        Me.Defence = Defence

    End Sub

    Private Sub AssignId()
        Guid = System.Guid.NewGuid()
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

    Public ReadOnly Property Art As String
        Get

            Dim Images() As String = {">", "}>", "}->", "}=>", "}]=>"}
            Dim Size As Integer = Math.Floor(MaxHP / 20)

            If Size > 4 Then
                Size = 4
            End If

            Return Images(Size)

        End Get
    End Property

    Public Function FireOn(Target As Ship, AllShips As List(Of ShipOrgUnit)) As Boolean

        Console.WriteLine()
        Me.WriteName()
        Console.Write(" firing on ")
        Target.WriteName()
        Console.WriteLine("...")

        Dim Damages(3) As Integer
        Dim BestDamage As Integer = 0
        Dim BestWeapon As WeaponType = WeaponType.Laser

        For W As Integer = 0 To 2
            Dim ThisDamage As Integer = CInt(Me.Attack(W)) - Target.Defence(W)
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

        If (Target.NoHealth) Then

            Target.Die(AllShips)

            Console.Write("  ...")
            Target.WriteName()
            Console.WriteLine(" was destroyed!!")

            Return True

        End If

        Return False

    End Function

    Public Sub Die(AllShips As List(Of ShipOrgUnit))

        OrgUnit.RemoveShip(Me, AllShips)

    End Sub

    Public Sub WriteName() Implements ITransponding.WriteName

        If TypeOf Me.Owner Is Human Then
            ResetConsole()
        Else
            Console.ForegroundColor = Me.Owner.Race.Colour
        End If

        Console.Write(Me.Name)

        'Reset console defaults
        ResetConsole()

    End Sub
End Class
