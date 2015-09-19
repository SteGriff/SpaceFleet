Public Class Race
    Implements ITransponding

    Public Name As String
    Public Face As String
    Public Colour As ConsoleColor

    Public Sub New(Name As String, Face As String, Colour As ConsoleColor)
        Me.Name = Name
        Me.Face = Face
        Me.Colour = Colour

    End Sub

    Public Sub Introduce()

        Console.WriteLine("""Stranger! We are the {0} race! You will never take our systems from us!""", Me.Name)

    End Sub

    Public Sub DeathWail()

        Console.WriteLine("""Nooooooo!! This cannot be the end of the {0}!""", Me.Name)

    End Sub

    Public Sub DrawFace()

        Console.ForegroundColor = Colour
        Console.Write(Face)
        ResetConsole()

    End Sub

    Public Sub WriteName() Implements ITransponding.WriteName

        Console.ForegroundColor = Me.Colour
        Console.Write(Me.Name)
        ResetConsole()

    End Sub

End Class
