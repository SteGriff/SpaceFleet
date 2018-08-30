Option Strict On
Option Infer Off

Public Class ShipFactory

    Public Function ShipFromDesign(Design As String) As Ship

        Dim bulkheads As Integer = 0
        Dim weaponMode As WeaponType = WeaponType.None
        Dim armourMode As ArmourType = ArmourType.None
        Dim valence As Integer = 0

        For Each thisChar As Char In Design
            Select Case thisChar
                Case ">"c
                Case "<"c
                    bulkheads += 1
                    Exit Select
                Case "L"c
                Case "l"c
                    weaponMode = WeaponType.Laser
                    Exit Select
                Case "M"c
                Case "m"c
                    weaponMode = WeaponType.Missile
                    Exit Select
                Case "D"c
                Case "d"c
                    weaponMode = WeaponType.MassDriver
                    Exit Select
                Case Else
                    Dim charAsInt As Integer
                    If Int32.TryParse(CStr(thisChar), charAsInt) Then
                        valence = charAsInt
                    End If

            End Select
        Next

    End Function


End Class
