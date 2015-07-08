Public Class Star

    Public Name As String
    Public Index As Integer
    Public Location As Long
    Public PlanetsHaveGreekLetters As Boolean

    Public Sub New(Name As String, Index As Integer, GreekNaming As Boolean)
        Me.Name = Name
        Me.Index = Index
        Me.Location = Index * 10 * (Rnd(15) + 5)
        Me.PlanetsHaveGreekLetters = GreekNaming

    End Sub

End Class
