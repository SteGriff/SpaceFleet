Public Class Map

    Public Items As List(Of KeyValuePair(Of Integer, IConsoleEntity))

    Public Sub New()
        Me.Items = New List(Of KeyValuePair(Of Integer, IConsoleEntity))
    End Sub

    Public Sub Add(Key As Integer, Value As IConsoleEntity)
        Me.Items.Add(New KeyValuePair(Of Integer, IConsoleEntity)(Key, Value))
    End Sub

    Public Function ContainsKey(Key As Integer) As Boolean
        Return Me.Items.Any(Function(x) (x.Key = Key))
    End Function

    Public Function OrderedByKey() As List(Of KeyValuePair(Of Integer, IConsoleEntity))
        Return Me.Items.OrderBy(Of Integer)(Function(x) (x.Key)).ToList()
    End Function

End Class
