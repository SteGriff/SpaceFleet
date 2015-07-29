Public Class CommsMessage

    Public Sender As Race
    Public Kind As CommsKind

    Public Enum CommsKind
        Introduction
        DeathWail
    End Enum

    Public Sub New(Sender As Race, Kind As CommsKind)
        Me.Sender = Sender
        Me.Kind = Kind
    End Sub
End Class
