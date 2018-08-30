Namespace ShipMenus
    Module ShipMenus

        Sub ShipRoster(You As Human, Feedback As Action(Of String))

            Console.WriteLine("Now building the """ & You.CurrentlyBuilding.DesignName.ToUpper & """ design")
            Console.WriteLine("Ships are produced on " & You.ConstructionPlanet.Name)
            Console.WriteLine()

            'Console.WriteLine("[Laser/Driver/Missiles]")

            InvertConsole()
            ShipColumnHeaders()
            ResetConsole()

            For Each Item In You.ShipOrgs.SelectMany(Function(s) (s.Ships))
                Item.Info()
            Next

            Console.WriteLine()
            ShipMenu()

            Select Case UserChoice()

                Case ConsoleKey.P
                'Purchase

                Case ConsoleKey.L
                    'Location
                    SetShipBuildLocation(You.Planets, You.ConstructionPlanet, Feedback)

                Case ConsoleKey.D
                    'Designs
                    ShipDesigner(You.CurrentlyBuilding)

            End Select

        End Sub

        Sub ShipMenu()

            Console.WriteLine("[L]ocation for newly built ships")
            'Console.WriteLine("[P]urchase current ship instantly")
            Console.WriteLine("[D]esign ships")
            Console.WriteLine("[Return] Done")

        End Sub

        Private Sub SetShipBuildLocation(Planets As List(Of Planet), ByRef ConstructionPlanet As Planet, Feedback As Action(Of String))

            Dim SelectedPlanet = SelectPlanet(Planets)

            If SelectedPlanet Is Nothing Then
                Feedback("ships will continue to be built on " & ConstructionPlanet.Name)
                Return
            End If

            ConstructionPlanet = SelectedPlanet
            Feedback("ships will be built on " + ConstructionPlanet.Name)

        End Sub

        Private Sub ShipDesigner(CurrentlyBuilding As Ship)

            Console.WriteLine("To type out a ship design:")
            Console.WriteLine("Start with bulkheads. Type '<' once for each size unit you want to add")

        End Sub

        Sub ShipColumnHeaders()

            Console.WriteLine(Ship.InfoTemplate, "Name", "Warp", "Attack", "Defence", "HP/Max")

        End Sub

    End Module
End Namespace

