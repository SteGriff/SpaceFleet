Module Regex

    Structure Ship

        Dim Name As String
        Dim HP As Byte
        Dim MaxHP As Byte
        Dim Warp As Byte

        Dim LaserAttack As Byte
        Dim DriverAttack As Byte
        Dim MissileAttack As Byte

        Dim LaserDefence As Byte
        Dim DriverDefence As Byte
        Dim MissileDefence As Byte

        Sub New(ByVal Name As String, ByVal HP As Byte, ByVal Warp As Byte, ByVal LaserAttack As Byte, ByVal DriverAttack As Byte, ByVal MissileAttack As Byte, ByVal LaserDefence As Byte, ByVal DriverDefence As Byte, ByVal MissileDefence As Byte)

            Me.Name = Name
            Me.HP = HP
            Me.MaxHP = HP
            Me.Warp = Warp

            Me.LaserAttack = LaserAttack
            Me.LaserDefence = LaserDefence

            Me.DriverAttack = DriverAttack
            Me.DriverDefence = DriverDefence

            Me.MissileAttack = MissileAttack
            Me.MissileDefence = MissileDefence

        End Sub

    End Structure
    Enum PlanetBalance
        Neutral
        Research
        Economy
    End Enum
    Structure Planet

        Dim Name As String
        Dim Population As Byte '*10,000,000 = 0.1 billion to 25.6 billion people
        Dim Capacity As Byte
        Dim Research As Byte
        Dim Income As Byte
        Dim Resources As Byte
        Dim Balance As PlanetBalance

        Sub New(ByVal Name As String, ByVal Population As Byte, ByVal Capacity As Byte, ByVal TechnologyBenefit As Byte, ByVal MoneyBenefit As Byte, ByVal Resources As Byte)

            Me.Name = Name
            Me.Population = Population
            Me.Capacity = Capacity
            Me.Research = TechnologyBenefit
            Me.Income = MoneyBenefit
            Me.Resources = Resources
            Me.Balance = PlanetBalance.Neutral

        End Sub
        Function Stability() As Single
            Dim Result As Single = Math.Min(10, CInt(Me.Research) / 2 + CInt(Me.Income) / 2) / 10
            'MsgBox(Me.Name & " stb is " & CStr(Result))
            Return Result
        End Function
        Sub GrowPopulation()

            'Only if growth is needed
            If Me.Population <> Me.Capacity Then

                'We want to fill faster if there's more free space (fast expansion)
                Dim FreeSpace As Integer = (Me.Capacity - Me.Population)
                Dim FreeSpacePercent As Integer = (FreeSpace / Me.Capacity) * 100

                'Based on a good measure of productivity:
                Dim Stability As Single = Me.Stability()
                'Division by which can be seen as a hindering factor.

                'Let's get ready to calculate the figure
                Dim PopulationIncrease As Integer

                'Fill the last 10 percent instantly to avoid 'approaching' max.
                If FreeSpacePercent <= 5 Then
                    PopulationIncrease = FreeSpace
                Else
                    'Fill a fraction of the free space, accounting for world development level.
                    PopulationIncrease = (FreeSpace / 3) * Stability
                End If

                'Avoid growth stagnation
                'If PopulationIncrease < 1 Then
                'PopulationIncrease = 1
                'End If

                'Boil it down to the new population figure in the correct data type
                Dim NewPopulation As Byte
                'Make sure there's no logical problem with the new figure fitting in 256
                Try
                    NewPopulation = Population + PopulationIncrease
                Catch ex As Exception
                    NewPopulation = Me.Capacity
                End Try

                If NewPopulation > Capacity Then
                    NewPopulation = Capacity
                End If

                'Checkline!
                'Console.WriteLine("{0} grew from {1} to {2} in maximum {3}.", Me.Name, Me.Population, NewPopulation, Me.Capacity)
                'Console.WriteLine("{0} ({1}): MaxGrowth {2}; Stability {3}; ActualGrowth {4} -> {5}/{6}", Me.Name, Me.Population, (FreeSpace / 2), Stability, PopulationIncrease, NewPopulation, Me.Capacity)

                'Set!
                Me.Population = NewPopulation

            End If

        End Sub

        Sub GrowIncome()

            'Each member of the population uses the cumulative planet resources to make money this turn. Limit to 255.
            Dim NewIncome As Short = Math.Min(255, CShort(PopulationFormat(Me.Population / 2)) * CShort(Me.Resources))
            Me.Income = CByte(NewIncome)

        End Sub

        Sub GrowResearch()

            'A random amount of the planets resources is added to its current research ability. Limit to 255.
            Dim NewResearch As Short = Math.Min(255, Math.Max(Me.Research, (Me.Research - 1 + Int(Rnd() * Me.Resources))))
            Me.Research = CByte(NewResearch)

        End Sub

    End Structure

    Structure Technology

        Dim Field As TechnologyType
        Dim Level As Single

        Sub New(ByVal Field As TechnologyType, ByVal Level As Integer)

            Me.Field = Field
            Me.Level = Level

        End Sub

        Function Name()
            Return Me.Field.ToString
        End Function

        Function Improvement(ByVal TechIncome As Integer, ByVal Technology() As Technology)

            Dim Current As Single = Me.Level
            Dim Income As Single = CSng(Math.Sqrt(TechIncome)) / 20
            Dim Ability As Single = CSng(Math.Sqrt(Technology(TechnologyType.Research).Level))
            Dim Prospective As Single = Math.Round(Current + CSng(Income * Ability), 1)
            Return Math.Min(100, Prospective)

        End Function

        Sub Improve(ByVal TechIncome As Single, ByVal Technology() As Technology)
            Me.Level = Improvement(TechIncome, Technology)
        End Sub
    End Structure
    Enum TechnologyType

        Lasers
        Drivers
        Missiles

        Deflectors
        Plating
        Chaff

        Warp

        Finance
        Population
        Research

    End Enum

    Function PopulationFormat(ByVal aPopulationValue) As Single
        Return Math.Round(CSng(aPopulationValue) / 10, 1)
    End Function

    Sub Main()

        Randomize()

        Console.WriteLine(vbTab & "Space fleet!" & vbNewLine)
        Console.Write("Enter race name: ")
        Dim RaceName As String = Console.ReadLine

        Dim Money As Integer = 1000

        Dim Ship(1) As Ship
        Dim NumberOfShips As Integer = 1
        Ship(0) = New Ship(RaceName + " defence drone", 2, 2, 1, 0, 0, 1, 1, 1)
        Dim CurrentlyBuilding As Ship = Ship(0)

        Dim Planet(1) As Planet
        Dim NumberOfPlanets As Integer = 2
        Planet(0) = New Planet("Earth", 60, 160, 1, 1, 5)
        Planet(1) = New Planet("Mars", 10, 80, 1, 2, 1)

        'Initialise technologise. Make Research Lv1.
        Dim Technology(10) As Technology
        For This As TechnologyType = 0 To 9
            If This = TechnologyType.Research Then
                Technology(This) = New Technology(This, 1)
            Else
                Technology(This) = New Technology(This, 0)
            End If
        Next

        Dim Researching As TechnologyType = SpaceFleet.TechnologyType.Research

        Const TaxRate As Integer = 1

        Dim Turn As Integer = 0
        Dim GameOver As Boolean = False
        Dim TurnTaken As Boolean = True
        Do Until GameOver

            Dim TechIncome As Integer
            Dim CashIncome As Integer
            Dim Population As Integer
            Dim Capacity As Integer

            If TurnTaken Then

                'Do civilisation growth

                For ThisColony As Integer = 0 To NumberOfPlanets - 1
                    Planet(ThisColony).GrowPopulation()
                    Planet(ThisColony).GrowIncome()
                    Planet(ThisColony).GrowResearch()
                Next

                'Wait for checklines
                'Console.ReadLine()

                'Sum the new levels of everything

                TechIncome = TotaliseTech(Planet)
                CashIncome = TotaliseMoney(Planet)
                Population = TotalisePopulation(Planet)
                Capacity = TotaliseCapacity(Planet)

                'Update economy figures

                Technology(Researching).Improve(TechIncome, Technology)
                Money += (CashIncome * TaxRate)

                'Initialise turn

                Turn = Turn + 1
                TurnTaken = False

            End If

            'Dim CanSpendMoney As Boolean = (Money 
            Readout(Turn, Money, Population, Capacity, Researching, Technology)
            MainMenu()

            Dim Selection As ConsoleKey = UserChoice()

            Select Case Selection

                Case ConsoleKey.P

                    PlanetRoster(Planet)
                    PlanetMenu()

                    Dim PlanetMenuSelection As ConsoleKey = UserChoice()
                    Select Case PlanetMenuSelection

                        Case ConsoleKey.M

                            PlanetList(Planet)

                            Dim PlanetSelection As ConsoleKey = UserChoice()
                            Dim SelectedPlanetID As Short = KeyCodeToNumber(PlanetSelection)

                            If Planet(SelectedPlanetID).Balance = PlanetBalance.Neutral Then
                                Console.WriteLine("{0} has neutral production.", Planet(SelectedPlanetID).Name)
                            Else
                                Console.WriteLine("{0} is geared towards {1}.", Planet(SelectedPlanetID).Name, Planet(SelectedPlanetID).Balance.ToString.ToLower)
                            End If

                            PlanetManagementOptions()

                            Dim PlanetManagementSelection As ConsoleKey = UserChoice()

                            Select Case PlanetManagementSelection

                                Case ConsoleKey.X
                                    Exit Select

                                Case ConsoleKey.R
                                    Planet(SelectedPlanetID).Balance = PlanetBalance.Research
                                Case ConsoleKey.E
                                    Planet(SelectedPlanetID).Balance = PlanetBalance.Economy
                                Case ConsoleKey.N
                                    Planet(SelectedPlanetID).Balance = PlanetBalance.Neutral

                            End Select


                        Case ConsoleKey.X
                            Exit Select

                        Case Else

                    End Select
                    'Planet Roster

                Case ConsoleKey.S
                    ShipRoster(Ship)
                    'Ship Roster

                Case ConsoleKey.R

                    ResearchInformation(Technology, Researching, TechIncome)
                    ResearchMenu()
                    Dim ResearchSelection As ConsoleKey = UserChoice()
                    Select Case ResearchSelection
                        Case ConsoleKey.C
                            For ThisTechnology As Byte = 0 To NumberOfTechnologies()
                                Console.Write("[" & ThisTechnology & "] " & Technology(ThisTechnology).Field.ToString & vbTab)
                                Console.WriteLine(Technology(ThisTechnology).Level)
                            Next
                            Dim NewResearchField As Integer = ConvertNumberKey(UserChoice())
                            Researching = NewResearchField

                        Case ConsoleKey.X
                            Exit Select

                        Case Else
                            'Not an option

                    End Select
                    'Research

                Case ConsoleKey.B
                    Console.WriteLine("The latest design is " & CurrentlyBuilding.Name.ToUpper)
                    ShipColumnHeaders()
                    ShipInfo(CurrentlyBuilding)
                    ShipBuildMenu()
                    Dim ShipBuildSelection As ConsoleKey = UserChoice()
                    Select Case ShipBuildSelection
                        Case ConsoleKey.M


                        Case ConsoleKey.B Or ConsoleKey.P


                        Case ConsoleKey.X
                            Exit Select

                    End Select

                    'Build ship

                Case ConsoleKey.E
                    'Explore

                Case ConsoleKey.X
                    TurnTaken = True
                    'End turn

                Case Else
                    'Not an option

            End Select

            Console.WriteLine(vbNewLine & "Press Return")
            Console.ReadLine()

        Loop


    End Sub

    Function ConvertNumberKey(ByVal NumericalKey As ConsoleKey)

        Dim Result As Integer
        Result = CInt(Right(NumericalKey.ToString, 1))

        Return Result

    End Function

    Function UserChoice()

        Dim Selection As ConsoleKey = Console.ReadKey.Key
        Console.SetCursorPosition(0, Console.CursorTop - 1)
        Console.Write(vbCrLf & "-------" & vbCrLf)

        Return Selection

    End Function
    Function NumberOfTechnologies()
        Return ([Enum].GetValues(GetType(TechnologyType)).Length) - 1
    End Function

    Sub ResearchList()


    End Sub
    Sub ResearchInformation(ByVal Technology() As Technology, ByVal Researching As TechnologyType, ByVal TechIncome As Integer)

        Dim CurrentLevel As Single = LevelOfTechnologyField(Technology, Researching)
        Dim ProspectiveLevel As Integer = Technology(Researching).Improvement(TechIncome, Technology)
        Console.WriteLine("We're currently researching " & Researching.ToString.ToUpper)
        Console.WriteLine("The level to which we've researched it to date is " & CurrentLevel)
        Console.WriteLine("Next turn, it will be at level " & ProspectiveLevel)

    End Sub
    Function LevelOfTechnologyField(ByVal Technologies() As Technology, ByVal WhatField As TechnologyType)

        Return Technologies(WhatField).Level

    End Function

    Sub PlanetRoster(ByVal Planets() As Planet)

        Console.WriteLine("Name" & vbTab & "Pop/Max" & vbTab & "Research  Income Resources")

        For Each Item As Planet In Planets
            Console.WriteLine(Item.Name & vbTab & PopulationFormat(Item.Population) & "/" & PopulationFormat(Item.Capacity) & vbTab & Item.Research & vbTab & "  " & Item.Income & vbTab & " " & Item.Resources)
        Next

    End Sub
    Sub PlanetMenu()

        Console.WriteLine("[M]anage")
        Console.WriteLine("[X] Done")

    End Sub
    Sub PlanetManagementOptions()

        Console.WriteLine("[X] Cancel")
        Console.WriteLine("Gear [N]eutrally")
        Console.WriteLine("Gear towards [R]esearch")
        Console.WriteLine("Gear towards [E]conomy")

    End Sub
    Sub PlanetList(ByVal Planets() As Planet)

        For This As Short = 0 To Planets.Length - 1
            Console.WriteLine("[" & This & "] " & Planets(This).Name)
        Next

    End Sub
    Sub ShipBuildMenu()

        Console.WriteLine("[M]odify design")
        Console.WriteLine("[B]uy (or [P]urchase)")
        Console.WriteLine("[X] Done")

    End Sub
    Sub ShipRoster(ByVal Ships() As Ship)

        Console.WriteLine("[Laser/Driver/Missiles]")
        ShipColumnHeaders()
        For Each Item As Ship In Ships
            ShipInfo(Item)
        Next

    End Sub
    Sub ShipColumnHeaders()
        Console.WriteLine("Name" & vbTab & vbTab & "Warp" & vbTab & "Attack" & vbTab & "Defence" & vbTab & "HP/Max")
    End Sub
    Sub ShipInfo(ByVal aShip As Ship)

        Console.Write(aShip.Name & vbTab & aShip.Warp & vbTab)
        Console.Write(aShip.LaserAttack & "/" & aShip.DriverAttack & "/" & aShip.MissileAttack & vbTab)
        Console.Write(aShip.LaserDefence & "/" & aShip.DriverDefence & "/" & aShip.MissileDefence & vbTab)
        Console.Write(aShip.HP & "/" & aShip.MaxHP & vbNewLine)

    End Sub

    Sub MainMenu()

        Console.WriteLine("[P]lanet Roster")
        Console.WriteLine("[S]hip Roster")
        Console.WriteLine("Set [R]esearch task")
        Console.WriteLine("Set [B]uild ship task")
        Console.WriteLine("[E]xplore")
        Console.WriteLine("[X] End Turn")

    End Sub

    Sub ResearchMenu()

        Console.WriteLine("[C]hange research item")
        Console.WriteLine("[X] Done")

    End Sub
    Sub Readout(ByVal Turn As Integer, ByVal Money As Integer, ByVal Population As Integer, ByVal Capacity As Integer, ByVal Researching As TechnologyType, ByVal Technology() As Technology)

        Console.Clear()
        Console.Write(vbNewLine & vbTab)
        Console.Write("Turn " & Turn & " | ")
        Console.Write("¤ " & Money & " | ")
        Console.WriteLine(PopulationFormat(Population) & "/" & PopulationFormat(Capacity) & " billion People")

        Console.Write(vbTab)
        Console.WriteLine("Improving " & Researching.ToString & " (Lv " & Technology(Researching).Level & ")")

        Console.WriteLine()

    End Sub


    Function TotalisePopulation(ByVal Planets() As Planet)
        Dim Result As Integer = 0

        For Each Item As Planet In Planets
            Result += Item.Population
        Next

        Return Result
    End Function

    Function TotaliseCapacity(ByVal Planets() As Planet)
        Dim Result As Integer = 0

        For Each Item As Planet In Planets
            Result += Item.Capacity
        Next

        Return Result
    End Function

    Function TotaliseMoney(ByVal Planets() As Planet)
        Dim Result As Integer = 0

        For Each Item As Planet In Planets
            Result += Item.Income
        Next

        Return Result
    End Function

    Function TotaliseTech(ByVal Planets() As Planet)
        Dim Result As Integer = 0

        For Each Item As Planet In Planets
            Result += Item.Research
        Next

        Return Result
    End Function
    Function KeyCodeToNumber(ByVal aKeyCode As ConsoleKey)
        Dim JustNumberCharacter As String = Right(aKeyCode.ToString, 1)
        Dim JustNumber As Short = CInt(JustNumberCharacter)
        Return JustNumber
    End Function
End Module
