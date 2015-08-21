Public Class SelectionModel
    Public Number As Integer
    Public Command As String
    Public Cancelled As Boolean
    Public Entity As IConsoleEntity

    Public Sub New()
        Me.Number = -1
        Me.Cancelled = False
        Me.Command = String.Empty
    End Sub

    Public Sub New(Number As Integer, Cancel As Boolean)
        Me.Number = Number
        Me.Cancelled = Cancel
        Me.Command = String.Empty
    End Sub

    Public Sub Cancel()
        Me.Cancelled = True
    End Sub

    Public ReadOnly Property HasCommand As Boolean
        Get
            Return Command <> String.Empty
        End Get
    End Property

End Class
