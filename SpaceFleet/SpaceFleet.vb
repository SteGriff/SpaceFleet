Option Strict On

Module SpaceFleet

    Dim Randomiser As Random

    Sub Main()

        Randomize()
        Randomiser = New Random()

        'Race init
        Console.WriteLine(vbTab & "Space fleet!" & vbNewLine)
        Console.Write("Enter race name: ")
        Dim RaceName As String = Console.ReadLine

        Dim Money As Integer = 1000

        'Ship and Ship Design init
        Dim Ships As New List(Of Ship)
        Dim ShipDesigns(0) As Ship

        Dim Attack As Byte() = {1, 0, 0}
        Dim Defence As Byte() = {1, 1, 1}

        ShipDesigns(0) = New Ship(RaceName + " defence drone", 2, 2, Attack, Defence)
        Dim CurrentlyBuilding As Ship = ShipDesigns(0)
        Dim ProductionPoints As Decimal = 0

        Ships.Add(CType(ShipDesigns(0).Clone(), Ship))

        'Planet init
        Dim MyPlanets(1) As MyPlanet
        MyPlanets(0) = New MyPlanet("Earth", 6, 16, 1, 1, 5, "O", ConsoleColor.Blue)
        MyPlanets(1) = New MyPlanet("Mars", 1, 8, 1, 2, 1, "o", ConsoleColor.DarkRed)

        Dim PG As New PlanetGeneration(Randomiser)
        Dim Planets() As Planet = PG.GeneratePlanets()

        'Initialise technologise, all Lv1.
        Dim Technologies(10) As Technology
        For This As TechnologyType = 0 To CType(9, TechnologyType)
            Technologies(This) = New Technology(This, 1)
        Next

        Dim Researching As TechnologyType = TechnologyType.Research

        Const TaxRate As Integer = 1

        Dim Week As Integer = 0
        Dim GameOver As Boolean = False
        Dim TurnTaken As Boolean = True

        Do Until GameOver

            Dim Totals As SystemTotals

            If TurnTaken Then

                'Do civilisation growth
                Dim PopulationGrowth As Decimal = 0

                For ThisPlanet As Integer = 0 To MyPlanets.Length - 1
                    PopulationGrowth += MyPlanets(ThisPlanet).GrowPopulation()
                    MyPlanets(ThisPlanet).GrowIncome()
                    MyPlanets(ThisPlanet).GrowResearch()
                Next

                'Sum the new levels of everything (for reporting)

                Totals = Totalise(MyPlanets)

                'Update production
                ProductionPoints += Totals.ProductionIncome

                Dim ShipJustBuilt As Boolean = False
                'Production points have satisfied current build job
                If (ProductionPoints > CurrentlyBuilding.Complexity) Then
                    'Gain the ship by cloning the design into the ship roster
                    Ships.Add(CType(CurrentlyBuilding.Clone(), Ship))

                    'Calculate leftover production pts
                    ProductionPoints -= CurrentlyBuilding.Complexity
                    ShipJustBuilt = True
                End If

                'Update technology and get level-up flag
                Technologies(Researching).ImproveAndCheckAdvancement(Totals.TechIncome, Technologies)
                Money = CInt(Money + (Totals.CashIncome * TaxRate))

                'Display to player
                Readout(Week, Money, Totals, Researching, Technologies, CurrentlyBuilding, ProductionPoints)

                If Week > 0 Then
                    'Give weekly report if this is not the first turn
                    WeeklyReport(Week, Technologies(Researching), PopulationGrowth, Totals, ShipJustBuilt, CurrentlyBuilding, ProductionPoints)
                End If

                'Initialise turn

                Week = Week + 1
                TurnTaken = False

            End If

            Dim CanSpendMoney As Boolean = (Money > 0)

            Readout(Week, Money, Totals, Researching, Technologies, CurrentlyBuilding, ProductionPoints)
            MainMenu()

            Dim Selection As ConsoleKey = UserChoice()

            Select Case Selection

                Case ConsoleKey.P
                    PlanetRoster(MyPlanets)

                Case ConsoleKey.S
                    ShipRoster(Ships.ToArray(), CurrentlyBuilding)

                Case ConsoleKey.R
                    ResearchManagement(Technologies, Researching, Totals.TechIncome)

                Case ConsoleKey.O
                    'orders
                    OrdersManagement(Planets)
                    Console.ReadLine()

                Case ConsoleKey.D
                    'diplomacy

                Case ConsoleKey.X
                    'End turn
                    TurnTaken = True

                Case Else
                    'Not an option

            End Select

        Loop


    End Sub

    Function UserChoice() As ConsoleKey

        Dim Selection As ConsoleKey = Console.ReadKey.Key
        Console.SetCursorPosition(0, Console.CursorTop - 1)
        Console.Write(vbCrLf & "-------" & vbCrLf)

        Return Selection

    End Function

    Function NumberOfTechnologies() As Short
        Return CShort([Enum].GetValues(GetType(TechnologyType)).Length) - 1S
    End Function

    Private Sub Feedback(Text As String)
        Console.WriteLine("Ok, " & Text)
        PressReturn()
    End Sub

    Private Sub DrawMap(Planets() As Planet)

        For Each P As Planet In Planets
            P.Draw()
            Console.WriteLine()
        Next

    End Sub

#Region "Orders"

    Private Sub OrdersManagement(Planets() As Planet)

        DrawMap(Planets)

    End Sub

#End Region

#Region "Planets"

    Private Sub PlanetManagement(ByRef Planets() As MyPlanet)

        PlanetSelectionList(Planets)

        Dim PlanetSelection As ConsoleKey = UserChoice()
        Dim SelectedPlanetID As Short = ConvertNumberKey(PlanetSelection)

        Console.WriteLine(Planets(SelectedPlanetID).FocusDescription)

        PlanetManagementOptions()

        Dim PlanetManagementSelection As ConsoleKey = UserChoice()

        Select Case PlanetManagementSelection

            Case ConsoleKey.X
                Exit Select

            Case ConsoleKey.R
                Planets(SelectedPlanetID).Focus = ProductionFocus.Research
                Feedback(Planets(SelectedPlanetID).FocusDescription)

            Case ConsoleKey.P
                Planets(SelectedPlanetID).Focus = ProductionFocus.Production
                Feedback(Planets(SelectedPlanetID).FocusDescription)

            Case ConsoleKey.B
                Planets(SelectedPlanetID).Focus = ProductionFocus.Balanced
                Feedback(Planets(SelectedPlanetID).FocusDescription)

        End Select

    End Sub

    Sub PlanetRoster(ByVal Planets() As MyPlanet)

        Const Template As String = "{0,-16}{1,-10}{2,-9}{3,-8}{4,-6}{5,-9}"

        Console.WriteLine(Template, "Name", "Pop/Max", "Research", "Income", "Rsrcs", "Focus")

        For Each Item As MyPlanet In Planets
            Console.WriteLine(Template, Item.Name, PopulationFormat(Item.Population) & "/" & PopulationFormat(Item.Capacity), Item.Research, Item.Income, Item.Resources, Item.Focus)
        Next

        PlanetMenu()

        Dim PlanetMenuSelection As ConsoleKey = UserChoice()
        Select Case PlanetMenuSelection

            Case ConsoleKey.M
                PlanetManagement(Planets)

        End Select

    End Sub
    Sub PlanetMenu()

        Console.WriteLine("[M]anage")
        Console.WriteLine("[X] Done")

    End Sub
    Sub PlanetManagementOptions()

        Console.WriteLine("[X] Cancel")
        Console.WriteLine("[B]alanced focus")
        Console.WriteLine("[R]esearch focus")
        Console.WriteLine("[P]roduction focus")

    End Sub
    Sub PlanetSelectionList(ByVal Planets() As MyPlanet)

        For This As Short = 0 To CShort(Planets.Length - 1)
            Console.WriteLine("[" & This & "] " & Planets(This).Name)
        Next

    End Sub
#End Region

#Region "Ships"

    Sub ShipMenu()

        Console.WriteLine("[P]urchase current ship instantly")
        Console.WriteLine("[D]esign ships")
        Console.WriteLine("[X] Done")

    End Sub
    Sub ShipRoster(ByVal Ships() As Ship, CurrentlyBuilding As Ship)

        Console.WriteLine("Now building the """ & CurrentlyBuilding.DesignName.ToUpper & """ design")
        Console.WriteLine()

        Console.WriteLine("FLEET")
        Console.WriteLine("(Attack/Defence given as: Laser/Driver/Missiles)")

        ShipColumnHeaders()
        For Each Item As Ship In Ships
            Item.Info()
        Next

        ShipMenu()

        Select Case UserChoice()

            Case ConsoleKey.P
                'Purchase


            Case ConsoleKey.D
                'Designs
                ShipDesigner(CurrentlyBuilding)

        End Select

    End Sub

    Private Sub ShipDesigner(CurrentlyBuilding As Ship)



    End Sub

    Sub ShipColumnHeaders()

        Console.WriteLine(Ship.InfoTemplate, "Name", "Warp", "Attack", "Defence", "HP/Max")

    End Sub

#End Region

#Region "Research"

    Private Sub ResearchManagement(Technologies() As Technology, ByRef Researching As TechnologyType, TechIncome As Decimal)

        ResearchInformation(Technologies, Researching, TechIncome)

        ResearchMenu()

        Select Case UserChoice()

            Case ConsoleKey.C
                For ThisTechnology As Short = 0 To NumberOfTechnologies()
                    Console.Write("[" & ThisTechnology & "] " & Technologies(ThisTechnology).Field.ToString & vbTab)
                    Console.WriteLine(Technologies(ThisTechnology).Level)
                Next
                Dim NewResearchField As Integer = ConvertNumberKey(UserChoice())
                Researching = CType(NewResearchField, TechnologyType)

        End Select

    End Sub

    Sub ResearchList()

        Console.WriteLine("{Research list}")

    End Sub
    Sub ResearchInformation(ByVal Technologies() As Technology, ByVal Researching As TechnologyType, ByVal TechIncome As Decimal)

        Dim CurrentLevel As Decimal = Technologies(Researching).Level
        Dim ProspectiveLevel As Decimal = Technologies(Researching).Improvement(TechIncome, Technologies)

        Console.WriteLine("We're currently researching " & Researching.ToString.ToUpper)
        Console.WriteLine("The level to which we've researched it to date is " & CurrentLevel)
        Console.WriteLine("Next week, it will be at level " & ProspectiveLevel)

    End Sub

    Sub ResearchMenu()

        Console.WriteLine("[C]hange research item")
        Console.WriteLine("[X] Done")

    End Sub

#End Region

    Sub MainMenu()

        Console.WriteLine("[P]lanets")
        Console.WriteLine("[S]hips")
        Console.WriteLine("[R]esearch")
        Console.WriteLine("[O]rders")
        Console.WriteLine("[D]iplomacy")
        Console.WriteLine("[X] End Week")

    End Sub

    Sub Readout(ByVal Turn As Integer, ByVal Money As Integer, ByVal Totals As SystemTotals, ByVal Researching As TechnologyType, ByVal Technology() As Technology, CurrentlyBuilding As Ship, ProductionPoints As Decimal)

        Console.Clear()
        Console.WriteLine(vbNewLine & vbTab & "Week {0} | ¤ {1} | {2}/{3} billion people", _
                          Turn, _
                          Money, _
                          PopulationFormat(Totals.Population), _
                          PopulationFormat(Totals.Capacity))

        Console.WriteLine(vbTab & "Improving {0} (Lv {1})", Researching.ToString(), Technology(Researching).Level)
        Console.WriteLine(vbTab & "Building a {0} ({1})", CurrentlyBuilding.DesignName, CurrentlyBuilding.PercentBuilt(ProductionPoints))
        Console.WriteLine()

    End Sub

    Function Totalise(ByVal Planets() As MyPlanet) As SystemTotals
        Dim Pop, Cap, Tech, Cash, Prod As Decimal

        For Each P As MyPlanet In Planets
            Pop += P.Population
            Cap += P.Capacity
            Cash += P.Income
            Tech += P.Research
            Prod += P.Production
        Next

        Return New SystemTotals(Pop, Cap, Cash, Tech, Prod)
    End Function

    Private Sub PressReturn()
        Console.WriteLine(vbNewLine & "Press Return")
        Console.ReadLine()
    End Sub

    Private Sub WeeklyReport(Week As Integer, Technology As Technology, PopulationChange As Decimal, Totals As SystemTotals, ShipJustBuilt As Boolean, CurrentlyBuilding As Ship, ProductionPoints As Decimal)

        Console.WriteLine("Governer's report for week {0}", Week)

        Console.WriteLine("We gained {0} billion citizens", PopulationFormat(PopulationChange))
        Console.WriteLine("Our cash revenue was ¤{0}", Totals.CashIncome)

        Console.Write("Our technological progress was {0} pts", Totals.TechIncome)

        If Technology.LevelledUpThisWeek Then
            Console.WriteLine(", and we achieved {0} level {1}!", Technology.Name, Technology.Level)
        End If

        Console.WriteLine()
        Console.WriteLine("Our production was {0} pts for a new total of {1}", Totals.ProductionIncome, ProductionPoints)

        If ShipJustBuilt Then
            Console.WriteLine("As we completed construction of '{0}'!", CurrentlyBuilding.Name)
        End If

        PressReturn()

    End Sub

End Module
