﻿Option Strict On

Module SpaceFleet

    ' Working title
    '   SPACE FLEET
    ' Or, "Hapless Galaxy", or "Unreasonable Space Conquest"
    ' This is a cheap console game drawing on Galactic Civilisations II
    ' It's a simplified 4X game of taking over the galaxy by force
    ' Stephen Griffiths 2011 - 2015

    Dim Randomiser As Random
    Dim Technologies(10) As Technology

    Sub Main()

        Randomize()
        Randomiser = New Random()

        'Race init
        Console.WriteLine(vbTab & "Space fleet!" & vbNewLine)
        Console.Write("Enter race name: ")
        Dim RaceName As String = Console.ReadLine

        Dim HumanRace As New Race(RaceName, "(>_<)", ConsoleColor.White)
        Dim You As New Human(HumanRace)

        'Initialise friendly planets
        Dim Planets As New List(Of Planet)
        Planets.Add(New MyPlanet("Earth", 0, 6, 16, 1, 1, 5, "O", ConsoleColor.Blue))
        Planets.Add(New MyPlanet("Mars", 1, 1, 8, 1, 2, 1, "o", ConsoleColor.DarkRed))

        'Initialise foreign stars and planets
        Dim Stars As New List(Of Star)
        Dim PlanetGenerator As New PlanetGeneration(Randomiser)
        Dim StrangePlanets = PlanetGenerator.GeneratePlanets(Stars)

        'Consolidate all planets in single array
        Planets = Planets.Concat(StrangePlanets).ToList()

        'Which planet do ships come out of (by Planet index)
        Dim ConstructionPlanet As Planet = Planets(0)

        'Generate enemy races
        Dim EnemyGenerator As New EnemyGeneration(Randomiser)
        Dim Enemies = EnemyGenerator.GenerateEnemies(Stars)

        'Create all techs and set to Lv1.
        InitialiseTechnologies(Technologies)

        Dim Researching As TechnologyType = TechnologyType.Research

        Const TaxRate As Integer = 1

        Dim Week As Integer = 0
        Dim GameOver As Boolean = False
        Dim TurnTaken As Boolean = True
        Dim Messages As New List(Of CommsMessage)

        Do Until GameOver

            Dim Totals As SystemTotals

            If TurnTaken Then

                'Do civilisation growth
                Dim PopulationGrowth As Decimal = 0

                For Each P As Planet In Planets
                    If TypeOf P Is MyPlanet Then
                        PopulationGrowth += DirectCast(P, MyPlanet).GrowPopulation()
                        DirectCast(P, MyPlanet).GrowIncome()
                        DirectCast(P, MyPlanet).GrowResearch()
                    End If
                Next

                'Sum the new levels of everything (for reporting)

                Totals = Totalise(Planets)

                'Update production
                You.ProductionPoints += Totals.ProductionIncome

                Dim ShipJustBuilt As Boolean = False
                'Production points have satisfied current build job
                If (You.ProductionPoints >= You.CurrentlyBuilding.Complexity) Then

                    'Get space-location of the planet where it was built
                    Dim BuildLocation As Integer = ConstructionPlanet.Location

                    'Gain the ship by cloning the design into the ship roster
                    You.Ships.Add(CType(You.CurrentlyBuilding.BuildClonedInstance(BuildLocation), Ship))

                    'Calculate leftover production pts
                    You.ProductionPoints -= You.CurrentlyBuilding.Complexity
                    ShipJustBuilt = True
                End If

                'Update technology and get level-up flag
                Technologies(Researching).ImproveAndCheckAdvancement(Totals.TechIncome, Technologies)
                You.Money = CInt(You.Money + (Totals.CashIncome * TaxRate))

                'Move all moving ships
                For Each S As Ship In You.Ships
                    S.Move()

                    'Forgive us our O(n^2) as we forgive those...
                    For Each E As Enemy In Enemies

                        If E.HasInTerritory(S) Then

                            E.Meet()

                            'Check if first contact
                            If E.Meetings = 1 Then
                                Messages.Add(New CommsMessage(E.Race, CommsMessage.CommsKind.Introduction))
                            End If

                        End If

                    Next

                    'TODO check whether you're in a different race zone to last turn
                    'N.b. fields on the Ship storing thisZone and lastZone?
                    'Queue introduction message if new encounter
                Next

                'Done with weekly jobs
                'Display to player
                Readout(Week, You.Money, Totals, Researching, Technologies, You.CurrentlyBuilding, You.ProductionPoints)

                If Week > 0 Then

                    CommsReport(Messages)

                    'Give weekly report if this is not the first turn
                    WeeklyReport(Week, Technologies(Researching), PopulationGrowth, Totals, ShipJustBuilt, You.CurrentlyBuilding, You.ProductionPoints)

                End If

                'Initialise turn

                Week = Week + 1
                TurnTaken = False

            End If

            Readout(Week, You.Money, Totals, Researching, Technologies, You.CurrentlyBuilding, You.ProductionPoints)
            MainMenu()

            Dim Selection As ConsoleKey = UserChoice()

            Select Case Selection

                Case ConsoleKey.P
                    PlanetRoster(Planets)

                Case ConsoleKey.S
                    ShipRoster(You.Ships.ToArray(), You.CurrentlyBuilding, Planets, ConstructionPlanet)

                Case ConsoleKey.R
                    ResearchManagement(Technologies, Researching, Totals.TechIncome)

                Case ConsoleKey.O
                    'orders
                    OrdersManagement(Planets, You.Ships.ToArray)

                Case ConsoleKey.D
                    'debug
                    If Debugger.IsAttached Then
                        DebugStuff(Enemies)
                    End If

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
        Divider()

        Return Selection

    End Function

    Sub Divider()
        Console.Write(vbCrLf & "-------" & vbCrLf)
    End Sub

    Function NumberOfTechnologies() As Short
        Return CShort([Enum].GetValues(GetType(TechnologyType)).Length) - 1S
    End Function

    Private Sub Feedback(Text As String)
        Console.WriteLine("Ok, " & Text)
        PressReturn()
    End Sub

    Private Function ShipLocationDelegate(S As Ship) As Integer
        Return S.Location
    End Function

    Private Function KeySelectionDelegate(kvp As KeyValuePair(Of Integer, Object)) As Integer
        Return kvp.Key
    End Function

    Private Function DrawMap(Planets As List(Of Planet), Ships() As Ship) As Dictionary(Of Integer, IConsoleEntity)

        'TODO Future feature - Sensors technology affect range
        'Dim SensorRange As Integer = Technologies(TechnologyType.Sensors) * 10
        Dim SensorRange As Integer = 100

        Dim FurthestExploredReach As Integer = Ships.Max(Function(s) (s.Location)) + SensorRange
        Dim Entities As New Map()
        Dim EntityMapping As New Dictionary(Of Integer, IConsoleEntity)

        Console.WriteLine("Sensor range: {0} parsecs (pc)", SensorRange)
        Console.WriteLine("Furthest Explored Reach: {0}pc", FurthestExploredReach)
        Console.WriteLine()

        For Each P As Planet In Planets

            If P.Location > FurthestExploredReach Then
                Exit For
            End If

            Entities.Add(P.Location, P)
        Next

        For Each S As Ship In Ships
            Entities.Add(S.Location, S)
        Next

        Dim EntityNumber As Integer = 0

        For Each E In Entities.OrderedByKey

            Console.Write("{0,-8}", "[" & EntityNumber & "]")
            EntityMapping.Add(EntityNumber, E.Value)
            EntityNumber += 1

            E.Value.Draw()

        Next

        Return EntityMapping

    End Function

#Region "Orders"

    Private Sub OrdersManagement(Planets As List(Of Planet), Ships() As Ship)

        Dim EntityMapping = DrawMap(Planets, Ships)

        Console.WriteLine()
        Console.WriteLine("Select a ship to move")
        Dim SelectedShip = SelectShipOnMap(EntityMapping)

        If SelectedShip Is Nothing Then
            Feedback("no changes made")
            Return
        End If

        Console.WriteLine()
        Console.WriteLine("Select a destination")
        Dim SelectedEntity As IConsoleEntity = SelectAnythingOnMap(EntityMapping)

        If SelectedEntity.Name = "" Then
            Feedback("no changes made")
            Return
        End If

        SelectedShip.Destination = SelectedEntity.Location
        Feedback(String.Format("sending {0} en route to {1}pc", SelectedShip.Name, SelectedEntity.Location))

    End Sub


    Private Function SelectAnythingOnMap(EntityMapping As Dictionary(Of Integer, IConsoleEntity)) As IConsoleEntity

        Dim SelectAnything As Type = GetType(IConsoleEntity)

        If EntityMapping.Count > 0 Then

            Dim Selection = GetNumber("Enter [number]: ")

            If Selection.Cancelled Then
                Return New Ship()
            End If

            Do Until EntityMapping.ContainsKey(Selection.Number)
                Selection = GetNumber("No such thing. Enter a number from square brackets above: ")
            Loop

            Return EntityMapping(Selection.Number)

        End If

        Console.WriteLine("Nothing to select")
        Return Nothing

    End Function

    Private Function SelectShipOnMap(EntityMapping As Dictionary(Of Integer, IConsoleEntity)) As Ship

        If EntityMapping.Count > 0 Then

            Dim ShipSelection = GetNumber("Enter [ship number] or 'x' to cancel: ")

            If ShipSelection.Cancelled Then
                Return Nothing
            End If

            Do Until EntityMapping.ContainsKey(ShipSelection.Number) AndAlso TypeOf EntityMapping(ShipSelection.Number) Is Ship
                ShipSelection = GetNumber("No such ship. Enter a [ship number] from above or'x': ")
            Loop

            Return CType(EntityMapping(ShipSelection.Number), Ship)

        End If

        Console.WriteLine("You have no ships")
        Return Nothing

    End Function

#End Region

#Region "Planets"

    Private Sub PlanetManagement(Planets As List(Of Planet))

        'Select a planet and cast it to MyPlanet
        Dim SelectedPlanet As MyPlanet = DirectCast(SelectPlanet(Planets), MyPlanet)

        If SelectedPlanet Is Nothing Then
            Feedback("nothing to do here...")
            Return
        End If

        Divider()

        'Report current focus
        Console.WriteLine(SelectedPlanet.FocusDescription)

        PlanetManagementOptions()

        Dim PlanetManagementSelection As ConsoleKey = UserChoice()

        Select Case PlanetManagementSelection

            Case ConsoleKey.X
                Return

            Case ConsoleKey.R
                SelectedPlanet.Focus = ProductionFocus.Research

            Case ConsoleKey.P
                SelectedPlanet.Focus = ProductionFocus.Production

            Case ConsoleKey.B
                SelectedPlanet.Focus = ProductionFocus.Balanced

        End Select

        Feedback(SelectedPlanet.FocusDescription)

    End Sub

    Sub PlanetRoster(Planets As List(Of Planet))

        Const Template As String = "{0,-16}{1,-10}{2,-9}{3,-8}{4,-6}{5,-9}"
        Console.WriteLine(Template, "Name", "Pop/Max", "Research", "Income", "Rsrcs", "Focus")

        Dim MyPlanets = Planets.Where(Function(P) (TypeOf P Is MyPlanet)).ToList()

        For Each Item As MyPlanet In MyPlanets
            Console.WriteLine(Template, Item.Name, PopulationFormat(Item.Population) & "/" & PopulationFormat(Item.Capacity), Item.Research, Item.Income, Item.Resources, Item.Focus)
        Next

        PlanetMenu()

        Dim PlanetMenuSelection As ConsoleKey = UserChoice()
        Select Case PlanetMenuSelection

            Case ConsoleKey.M
                PlanetManagement(MyPlanets)

        End Select

    End Sub
    Sub PlanetMenu()

        Console.WriteLine("[M]anage")
        Console.WriteLine("[X] Done")

    End Sub
    Sub PlanetManagementOptions()

        Console.WriteLine("[B]alanced focus")
        Console.WriteLine("[R]esearch focus")
        Console.WriteLine("[P]roduction focus")
        Console.WriteLine("[X] Cancel")

    End Sub
    Function DrawPlanetList(Planets As List(Of Planet)) As Dictionary(Of Integer, IConsoleEntity)

        Dim EntityMapping As New Dictionary(Of Integer, IConsoleEntity)

        For This As Short = 0 To CShort(Planets.Count - 1)
            EntityMapping.Add(This, Planets(This))
            Console.WriteLine("[" & This & "] " & Planets(This).Name)
        Next

        Return EntityMapping

    End Function
    Function SelectPlanet(Planets As List(Of Planet)) As Planet

        Planets = Planets.Where(Function(P) (TypeOf P Is MyPlanet)).ToList()
        Dim EntityMapping = DrawPlanetList(Planets)

        If EntityMapping.Count > 0 Then

            Dim Selection = GetNumber("Enter [number] or 'x' to cancel: ")

            If Selection.Cancelled Then
                Return Nothing
            End If

            Do Until EntityMapping.ContainsKey(Selection.Number) AndAlso TypeOf EntityMapping(Selection.Number) Is Planet
                Selection = GetNumber("No such planet. Enter a [number] from above or'x': ")
            Loop

            Return CType(EntityMapping(Selection.Number), Planet)

        End If

        Console.WriteLine("You have no planets")
        Return Nothing

    End Function

#End Region

#Region "Ships"

    Sub ShipMenu()

        Console.WriteLine("[L]ocation for newly built ships")
        Console.WriteLine("[P]urchase current ship instantly")
        Console.WriteLine("[D]esign ships")
        Console.WriteLine("[X] Done")

    End Sub
    Sub ShipRoster(ByVal Ships() As Ship, CurrentlyBuilding As Ship, Planets As List(Of Planet), ByRef ConstructionPlanet As Planet)

        Console.WriteLine("Now building the """ & CurrentlyBuilding.DesignName.ToUpper & """ design")
        Console.WriteLine("Ships are produced on " & ConstructionPlanet.Name)
        Console.WriteLine()

        Console.WriteLine("----- FLEET ----- ")
        'Console.WriteLine("[Laser/Driver/Missiles]")
        Console.WriteLine()

        ShipColumnHeaders()
        For Each Item As Ship In Ships
            Item.Info()
        Next

        Console.WriteLine()
        ShipMenu()

        Select Case UserChoice()

            Case ConsoleKey.P
                'Purchase

            Case ConsoleKey.L
                'Location
                SetShipBuildLocation(Planets, ConstructionPlanet)

            Case ConsoleKey.D
                'Designs
                ShipDesigner(CurrentlyBuilding)

        End Select

    End Sub

    Private Sub SetShipBuildLocation(Planets As List(Of Planet), ByRef ConstructionPlanet As Planet)

        Dim SelectedPlanet = SelectPlanet(Planets)

        If SelectedPlanet Is Nothing Then
            Feedback("ships will continue to be built on " & ConstructionPlanet.Name)
            Return
        End If

        ConstructionPlanet = SelectedPlanet
        Feedback("ships will be built on " + ConstructionPlanet.Name)

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

    Sub InitialiseTechnologies(ByRef Techs As Technology())
        For ThisTech As TechnologyType = 0 To CType(9, TechnologyType)
            Techs(ThisTech) = New Technology(ThisTech, 1)
        Next
    End Sub

    Sub MainMenu()

        Console.WriteLine("[P]lanets")
        Console.WriteLine("[S]hips")
        Console.WriteLine("[R]esearch")
        Console.WriteLine("[O]rders")

        If Debugger.IsAttached Then
            Console.WriteLine("[D]ebug")
        End If

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

    Function Totalise(Planets As List(Of Planet)) As SystemTotals
        Dim Pop, Cap, Tech, Cash, Prod As Decimal

        Dim MyPlanets = Planets.Where(Function(P) (TypeOf P Is MyPlanet))

        For Each P As MyPlanet In MyPlanets
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

    Public Function GetNumber(Question As String) As NumberModel

        Console.Write(Question)
        Dim InputString = Console.ReadLine

        Dim InputModel As New NumberModel(0, False)
        Dim InputNumber As Integer = 0
        Dim Valid As Boolean = False

        Do Until Valid Or InputModel.Cancelled

            Valid = Int32.TryParse(InputString, InputNumber)

            If Valid Then

                InputModel.Number = InputNumber

            Else

                If InputString.ToLower() = "x" Then
                    InputModel.Cancel()
                Else
                    Console.Write("Invalid number, try again: ")
                    InputString = Console.ReadLine
                End If

            End If

        Loop

        Return InputModel

    End Function

    Private Sub DebugStuff(Enemies As List(Of Enemy))

        For Each E As Enemy In Enemies
            Console.ForegroundColor = E.Race.Colour
            Console.WriteLine("{0,8} {1,-20} {2}", E.Race.Face, E.Race.Name, E.Ability)
            Console.ForegroundColor = ConsoleColor.Gray
        Next

        Console.ReadLine()

    End Sub

    Private Sub CommsReport(Messages As List(Of CommsMessage))

        Dim MNum As Integer = 1
        Dim TotalM As Integer = Messages.Count

        For Each M As CommsMessage In Messages

            Console.Clear()
            Console.WriteLine()
            Console.WriteLine("--------[ o . . ]---[ COMMS ]----[ . o o ]--------")
            Console.WriteLine()
            Console.WriteLine("Message {0} of {1}:", MNum, TotalM)

            Console.WriteLine()
            M.Sender.DrawFace()
            Console.WriteLine()

            'Write actual message
            Select Case M.Kind
                Case CommsMessage.CommsKind.Introduction
                    M.Sender.Introduce()
                Case CommsMessage.CommsKind.DeathWail
                    M.Sender.DeathWail()
            End Select

            Console.WriteLine()
            Console.WriteLine("Press a key...")
            Console.ReadLine()
        Next

    End Sub

End Module
