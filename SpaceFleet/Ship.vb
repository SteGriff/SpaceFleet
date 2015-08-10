Public Class Ship
    Implements ICloneable, IConsoleEntity, IColourful

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

    Public Const InfoTemplate As String = "{0,-24}{1,-6}{2,-12}{3,-12}{4,-8}"

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

    Public ReadOnly Property Dead() As Boolean
        Get
            Return HP <= 0
        End Get
    End Property

    Function Clone() As Object Implements ICloneable.Clone

        Return New Ship(Me.DesignName, Me.HP, Me.Warp, Me.Attack, Me.Defence)

    End Function

    Function BuildClonedInstance(Owner As Player, AllShips As List(Of Ship)) As Ship

        Dim NewShip As Ship = Me.Clone()
        NewShip.Owner = Owner
        NewShip.Location = Owner.ConstructionPlanet.Location

        AllShips.Add(NewShip)

        Return NewShip

    End Function

    Public Sub WriteName() Implements IColourful.WriteName

        If TypeOf Me.Owner Is Human Then
            ResetConsole()
        Else
            Console.ForegroundColor = Me.Owner.Race.Colour
        End If

        Console.Write(Me.Name)

        'Reset console defaults
        ResetConsole()

    End Sub

    Public Sub Draw() Implements IConsoleEntity.Draw

        Dim LocationString As String = ""
        If Moving() Then
            'TODO show number of weeks left in transport
            LocationString = String.Format("{0}pc -> {1}pc", Me.Location, Me.Destination)
        Else
            LocationString = String.Format("{0}pc", Me.Location)
        End If

        If TypeOf Me.Owner Is Human Then
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = ConsoleColor.Black
        Else
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = Me.Owner.Race.Colour
        End If

        Console.Write("{0}{1}{2}", Me.Art(), vbTab, Me.Name)

        'Reset console defaults
        ResetConsole()

        Console.Write(" {0}pc", Me.Location)

        'Write destination if the ship has belongs to the player
        If Me.Destination <> Me.Location _
            AndAlso TypeOf Me.Owner Is Human Then
            Console.Write(" -> {0}pc", Me.Destination)
        End If

        'End the line
        Console.WriteLine()

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

    Public Sub Move(AllShips As List(Of Ship))

        'Can't move if in battle
        If Engaged OrElse Location = Destination Then
            Return
        End If

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

        Debug.WriteLineIf(Location < 50, String.Format("{0} moved from {1} to {2}pc", Me.Name, OldLocation, Location))

        'Check if we passed another ship
        For Each S As Ship In AllShips

            If S.Equals(Me) Then
                Continue For
            End If

            If S.IsBetween(OldLocation, Location) AndAlso S.IsEnemy(Me) Then
                'Stop at that location and engage in battle
                Me.Location = S.Location
                Me.Engaged = True
                S.Engaged = True
                Exit For
            End If

        Next

    End Sub

    Public Function IsBetween(A As Integer, B As Integer)

        If A > B Then
            Return A >= Me.Location AndAlso Me.Location >= B
        ElseIf A < B Then
            Return A <= Me.Location AndAlso Me.Location <= B
        Else
            'A == B
            Return A = Me.Location
        End If

    End Function

    Public Function IsEnemy(S As Ship) As Boolean

        Return S.Owner.GetType().Name <> Me.Owner.GetType().Name

    End Function

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

End Class
