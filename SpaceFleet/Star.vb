Public Class Star

    Public Name As String
    Public PotentialRaceNames As List(Of String)
    Public Index As Integer
    Public Location As Long
    Public PlanetsHaveGreekLetters As Boolean
    Public Planets As List(Of Planet)

    Public Sub New(Index As Integer, PlanetsHaveGreekLetters As Boolean)

        Me.Index = Index
        Me.Location = Index * 10 * (Rnd(15) + 5)
        Me.PlanetsHaveGreekLetters = PlanetsHaveGreekLetters
        Me.Planets = New List(Of Planet)
        Me.PotentialRaceNames = New List(Of String)

    End Sub

End Class
