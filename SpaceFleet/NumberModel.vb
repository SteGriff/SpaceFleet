Public Class NumberModel
    Public Number As Integer
    Public Cancelled As Boolean

    Public Sub New(Number As Integer, Cancel As Boolean)
        Me.Number = Number
        Me.Cancelled = Cancel
    End Sub

    Public Sub Cancel()
        Me.Cancelled = True
    End Sub

End Class
