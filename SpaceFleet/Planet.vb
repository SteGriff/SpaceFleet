Public Class Planet

    Public Name As String
    Public Population As Decimal ' x billion people
    Public Capacity As Decimal

    Public Resources As Byte

    Public Colour As ConsoleColor
    Public Art As String

    Public Sub New(Name As String, Population As Decimal, Capacity As Decimal, Resources As Byte, Colour As ConsoleColor, Art As String)
        Me.Name = Name
        Me.Population = Population
        Me.Capacity = Capacity
        Me.Resources = Resources
        Me.Colour = Colour
        Me.Art = Art
    End Sub

    Public Sub Draw()

        Console.ForegroundColor = Me.Colour
        Console.Write(Me.Art)
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine(vbTab & Me.Name)

    End Sub
End Class
