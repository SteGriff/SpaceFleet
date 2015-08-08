Option Strict On

Module SpaceFleet

    ' Working title
    '   SPACE FLEET
    ' Or, "Hapless Galaxy", or "Unreasonable Space Conquest"
    ' This is a cheap console game drawing on Galactic Civilisations II
    ' It's a simplified 4X game of taking over the galaxy by force
    ' Stephen Griffiths 2011 - 2015

    Dim Randomiser As Random
    'Dim Technologies(10) As Technology

    Sub Main()

        Randomize()
        Randomiser = New Random()

        'Race init
        Console.WriteLine(vbTab & "Space fleet!" & vbNewLine)
        Console.Write("Enter race name: ")
        Dim RaceName As String = Console.ReadLine

        'Master list of planets
        Dim Earth As New MyPlanet("Earth", 0, 6, 16, 1, 1, 5, "O", ConsoleColor.Blue)
        Dim Mars As New MyPlanet("Mars", 1, 1, 8, 1, 2, 1, "o", ConsoleColor.DarkRed)
        Dim Planets As New List(Of Planet)({Earth, Mars})

        'Init player model
        Dim PlayersFirstPlanets As New List(Of Planet)({Earth, Mars})

        'Initialise master ship list
        Dim AllShips As New List(Of Ship)

        'Home star for initial location
        Dim Sol As New Star(0, False)

        'Init human player basics
        Dim HumanRace As New Race(RaceName, "(>_<)", ConsoleColor.White)
        Dim You As New Human(HumanRace, Sol, PlayersFirstPlanets, AllShips)

        'Initialise foreign stars and planets
        Dim Stars As New List(Of Star)
        Dim PlanetGenerator As New PlanetGeneration(Randomiser)
        Dim StrangePlanets = PlanetGenerator.GeneratePlanets(Stars)

        'Consolidate all planets in single array
        Planets = Planets.Concat(StrangePlanets).ToList()

        'Generate enemy races
        Dim EnemyGenerator As New EnemyGeneration(Randomiser)
        Dim Enemies = EnemyGenerator.GenerateEnemies(Stars, AllShips)

        'Create all techs and set to Lv1.
        InitialiseTechnologies(You.Technologies)

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

                For Each P As Planet In You.Planets
                    'This is a belt and brace check... it really should be MyPlanet
                    ' if it's in You.Planets
                    If TypeOf P Is MyPlanet Then
                        PopulationGrowth += DirectCast(P, MyPlanet).GrowPopulation()
                        DirectCast(P, MyPlanet).GrowIncome()
                        DirectCast(P, MyPlanet).GrowResearch()
                    End If
                Next

                'Sum the new levels of everything (for reporting)
                Totals = Totalise(You.Planets)

                'Update production
                You.ProductionPoints += Totals.ProductionIncome

                Dim ShipJustBuilt As Boolean = You.TryBuildShip(AllShips)

                'Update technology and get level-up flag
                You.Technologies(You.Researching).ImproveAndCheckAdvancement(Totals.TechIncome, You.Technologies)
                You.Money = CInt(You.Money + (Totals.CashIncome * TaxRate))

                MoveAllShips(AllShips, Enemies, Messages)
                Debug.WriteLine("")

                'Done with weekly jobs
                'Display to player
                Readout(Week, You, Totals)

                If Week > 0 Then

                    If (Messages.Count > 0) Then
                        CommsReport(Messages)
                        Messages.Clear()

                        'Refresh screen
                        Readout(Week, You, Totals)
                    End If

                    'Give weekly report if this is not the first turn
                    WeeklyReport(Week, You, PopulationGrowth, Totals, ShipJustBuilt)

                End If

                'Initialise turn

                Week = Week + 1
                TurnTaken = False

            End If

            Readout(Week, You, Totals)
            MainMenu()

            Dim Selection As ConsoleKey = UserChoice()

            Select Case Selection

                Case ConsoleKey.P
                    PlanetRoster(You.Planets)

                Case ConsoleKey.S
                    ShipRoster(You)

                Case ConsoleKey.R
                    ResearchManagement(You, Totals.TechIncome)

                Case ConsoleKey.O
                    'orders
                    OrdersManagement(Planets, You.Ships, AllShips)

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
        Console.WriteLine()
        Console.WriteLine("--------")
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

    Private Function DrawMap(Planets As List(Of Planet), YourShips As List(Of Ship), AllShips As List(Of Ship)) As Dictionary(Of Integer, IConsoleEntity)

        'TODO Future feature - Sensors technology affect range
        'Dim SensorRange As Integer = Technologies(TechnologyType.Sensors) * 10
        Dim SensorRange As Integer = 60
        Dim FurthestExploredReach As Integer = YourShips.Max(Function(s) (s.Location)) + SensorRange

        Console.Clear()
        Console.WriteLine()
        Console.WriteLine("-------- ORDERS --------")
        Console.WriteLine()
        Console.WriteLine("Sensor range: {0} parsecs (pc)", SensorRange)
        Console.WriteLine("Furthest Explored Reach: {0}pc", FurthestExploredReach)
        Console.WriteLine()

        Dim Entities As New Map()
        Dim EntityMapping As New Dictionary(Of Integer, IConsoleEntity)

        'Add all visible planets to Entities
        For Each P As Planet In Planets

            If P.Location > FurthestExploredReach Then
                Exit For
            End If

            Entities.Add(P.Location, P)
        Next

        'Add all visible ships to Entities
        For Each S As Ship In AllShips.OrderBy(Function(x) (x.Location))

            If S.Location > FurthestExploredReach Then
                Exit For
            End If

            Entities.Add(S.Location, S)
        Next

        'Sort entities by Location (key) and output them
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

    Private Sub OrdersManagement(Planets As List(Of Planet), YourShips As List(Of Ship), AllShips As List(Of Ship))

        Do

            Dim EntityMapping = DrawMap(Planets, YourShips, AllShips)

            Console.WriteLine()
            Console.WriteLine("SELECT SHIP")
            Dim SelectedShip = SelectShipOnMap(EntityMapping)

            If SelectedShip Is Nothing Then
                Return
            End If

            Console.WriteLine()
            Console.WriteLine("SELECT DESTINATION")
            Dim SelectedEntity As IConsoleEntity = SelectAnythingOnMap(EntityMapping)

            If SelectedEntity Is Nothing Then
                Return
            End If

            SelectedShip.Destination = SelectedEntity.Location

            Console.WriteLine()

            Console.WriteLine("Ok, sending {0} to {1}pc", SelectedShip.Name, SelectedEntity.Location)
            Console.WriteLine()
            Console.WriteLine("[A] Manage another")
            Console.WriteLine("[Anything else] Stop")
            Dim Choice As ConsoleKey = UserChoice()

            If Choice <> ConsoleKey.A Then
                Exit Do
            End If

        Loop

    End Sub


    Private Function SelectAnythingOnMap(EntityMapping As Dictionary(Of Integer, IConsoleEntity)) As IConsoleEntity

        Dim SelectAnything As Type = GetType(IConsoleEntity)

        If EntityMapping.Count > 0 Then

            Dim Selection = GetNumber("Enter [number] or press return to cancel: ")

            If Selection.Cancelled Then
                Return Nothing
            End If

            Do Until EntityMapping.ContainsKey(Selection.Number)
                Selection = GetNumber("No such thing. Enter [number] from above or press return: ")

                If Selection.Cancelled Then
                    Return Nothing
                End If
            Loop

            Return EntityMapping(Selection.Number)

        End If

        Console.WriteLine("Nothing to select")
        Return Nothing

    End Function

    Private Function SelectShipOnMap(EntityMapping As Dictionary(Of Integer, IConsoleEntity)) As Ship

        If EntityMapping.Count > 0 Then

            Dim ShipSelection = GetNumber("Enter [ship number] or press return to cancel: ")

            If ShipSelection.Cancelled Then
                Return Nothing
            End If

            Do Until EntityMapping.ContainsKey(ShipSelection.Number) _
                AndAlso TypeOf EntityMapping(ShipSelection.Number) Is Ship
                ShipSelection = GetNumber("No such ship. Enter [number] from above or press return: ")

                If ShipSelection.Cancelled Then
                    Return Nothing
                End If
            Loop

            Do Until TypeOf CType(EntityMapping(ShipSelection.Number), Ship).Owner Is Human
                ShipSelection = GetNumber("That ship isn't yours; try again: ")

                If ShipSelection.Cancelled Then
                    Return Nothing
                End If
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

            Dim Selection = GetNumber("Enter [number] or press return to cancel: ")

            If Selection.Cancelled Then
                Return Nothing
            End If

            Do Until EntityMapping.ContainsKey(Selection.Number) AndAlso TypeOf EntityMapping(Selection.Number) Is Planet
                Selection = GetNumber("No such planet. Enter [number] from above or press return: ")

                If Selection.Cancelled Then
                    Return Nothing
                End If
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
    Sub ShipRoster(You As Human)

        Console.WriteLine("Now building the """ & You.CurrentlyBuilding.DesignName.ToUpper & """ design")
        Console.WriteLine("Ships are produced on " & You.ConstructionPlanet.Name)
        Console.WriteLine()

        Console.WriteLine("-------- FLEET -------- ")
        'Console.WriteLine("[Laser/Driver/Missiles]")
        Console.WriteLine()

        ShipColumnHeaders()
        For Each Item As Ship In You.Ships
            Item.Info()
        Next

        Console.WriteLine()
        ShipMenu()

        Select Case UserChoice()

            Case ConsoleKey.P
                'Purchase

            Case ConsoleKey.L
                'Location
                SetShipBuildLocation(You.Planets, You.ConstructionPlanet)

            Case ConsoleKey.D
                'Designs
                ShipDesigner(You.CurrentlyBuilding)

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

    Private Sub ResearchManagement(You As Player, TechIncome As Decimal)

        ResearchInformation(You, TechIncome)

        ResearchMenu()

        Select Case UserChoice()

            Case ConsoleKey.C
                For ThisTechnology As Short = 0 To NumberOfTechnologies()
                    Console.Write("[" & ThisTechnology & "] " & You.Technologies(ThisTechnology).Field.ToString & vbTab)
                    Console.WriteLine(You.Technologies(ThisTechnology).Level)
                Next
                Dim NewResearchField As Integer = ConvertNumberKey(UserChoice())
                You.Researching = CType(NewResearchField, TechnologyType)

        End Select

    End Sub

    Sub ResearchList()

        Console.WriteLine("{Research list}")

    End Sub
    Sub ResearchInformation(You As Player, ByVal TechIncome As Decimal)

        Dim CurrentLevel As Decimal = You.Technologies(You.Researching).Level
        Dim ProspectiveLevel As Decimal = You.Technologies(You.Researching).Improvement(TechIncome, You.Technologies)

        Console.WriteLine("We're currently researching " & You.Researching.ToString.ToUpper)
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

    Sub Readout(ByVal Turn As Integer, You As Player, ByVal Totals As SystemTotals)

        Console.Clear()
        Console.WriteLine(vbNewLine & vbTab & "Week {0} | ¤ {1} | {2}/{3} billion people", _
                          Turn, _
                          You.Money, _
                          PopulationFormat(Totals.Population), _
                          PopulationFormat(Totals.Capacity))

        Console.WriteLine(vbTab & "Improving {0} (Lv {1})", You.Researching.ToString(), You.Technologies(You.Researching).Level)
        Console.WriteLine(vbTab & "Building a {0} ({1})", You.CurrentlyBuilding.DesignName, You.CurrentlyBuilding.PercentBuilt(You.ProductionPoints))
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

    Private Sub WeeklyReport(Week As Integer, You As Player, PopulationChange As Decimal, Totals As SystemTotals, ShipJustBuilt As Boolean)

        Dim Technology = You.Technologies(You.Researching)

        Console.WriteLine("Governer's report for week {0}", Week)

        Console.WriteLine("We gained {0} billion citizens", PopulationFormat(PopulationChange))
        Console.WriteLine("Our cash revenue was ¤{0}", Totals.CashIncome)

        Console.Write("Our technological progress was {0} pts", Totals.TechIncome)

        If Technology.LevelledUpThisWeek Then
            Console.WriteLine(", and we achieved {0} level {1}!", Technology.Name, Technology.Level)
        End If

        Console.WriteLine()
        Console.WriteLine("Our production was {0} pts for a new total of {1}", Totals.ProductionIncome, You.ProductionPoints)

        If ShipJustBuilt Then
            Console.WriteLine("As we completed construction of '{0}'!", You.CurrentlyBuilding.Name)
        End If

        PressReturn()

    End Sub

    Public Function GetNumber(Question As String) As NumberModel

        Console.Write("  {0}", Question)
        Dim InputString = Console.ReadLine

        Dim InputModel As New NumberModel(0, False)
        Dim InputNumber As Integer = 0
        Dim Valid As Boolean = False

        Do Until Valid Or InputModel.Cancelled

            Valid = Int32.TryParse(InputString, InputNumber)

            If Valid Then

                InputModel.Number = InputNumber

            Else

                If InputString = String.Empty Then
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

        Const Template As String = "{0,8} {1,-20} {2,-6} {3,-6} {4,-6}"
        Dim AllShips As New List(Of Ship)

        Console.WriteLine(Template, "Face", "Name", "Ablty", "TBgn", "TEnd")

        For Each E As Enemy In Enemies

            Console.ForegroundColor = E.Race.Colour
            Console.WriteLine(Template, E.Race.Face, E.Race.Name, E.Ability, E.TerritoryBegin, E.TerritoryEnd)
            ResetConsole()

            AllShips.AddRange(E.Ships)

        Next

        For Each S As Ship In AllShips.OrderBy(Function(x) (x.Location))
            Console.ForegroundColor = S.Owner.Race.Colour
            Console.WriteLine("{0,-6} {1,25}", S.Location, S.Name)
            ResetConsole()
        Next

        Console.ReadLine()

    End Sub

    Private Sub CommsReport(Messages As List(Of CommsMessage))

        Dim MNum As Integer = 1
        Dim TotalM As Integer = Messages.Count

        If TotalM = 0 Then
            Return
        End If

        Console.WriteLine()
        Console.WriteLine("-------- COMMS --------")
        Console.WriteLine()

        For Each M As CommsMessage In Messages

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

            MNum += 1

        Next

        Console.WriteLine("-----------------------")

    End Sub

    Private Sub MoveAllShips(AllShips As List(Of Ship), Enemies As List(Of Enemy), Messages As List(Of CommsMessage))

        'Move all moving ships
        For Each S As Ship In AllShips

            'Handle movement
            ' if battle is detected, 'Engaged' flag is set and involved ships come to stop
            S.Move(AllShips)

            If TypeOf S.Owner Is Human Then
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
            End If

        Next

        Dim Combatants = AllShips.Where(Function(x) (x.Engaged))

        Do While Combatants.Count > 0

            For Each S As Ship In Combatants

                'Handle battles
                Dim Battle As New SpaceBattle(S.Location)
                Battle.Fight(AllShips)
                
                'Collection modified, so we can't continue
                ' drop out into the Do-Loop and revise the collection
                Exit For

            Next

            Combatants = AllShips.Where(Function(x) (x.Engaged))

        Loop

    End Sub

End Module
