Module ResearchAdvisory

    ''' <summary>
    ''' Get the technology which counteracts the given weapon (if any)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function DefendAgainst(Weapon As WeaponType) As TechnologyType
        Select Case Weapon
            Case WeaponType.Laser
                Return TechnologyType.Deflectors
            Case WeaponType.MassDriver
                Return TechnologyType.Plating
            Case WeaponType.Missile
                Return TechnologyType.Chaff
            Case Else
                Return Nothing
        End Select
    End Function


End Module
