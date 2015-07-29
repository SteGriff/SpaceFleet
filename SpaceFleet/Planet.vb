Public Class Planet
    Implements IConsoleEntity

    Public MyName As String
    Public Population As Decimal ' x billion people
    Public Capacity As Decimal
    Public MyLocation As Integer

    Public Resources As Byte

    Public Colour As ConsoleColor
    Public Art As String

    Public Owner As IPlayer

    Public Property Location As Integer Implements IConsoleEntity.Location
        Get
            Return MyLocation
        End Get
        Set(value As Integer)
            MyLocation = value
        End Set
    End Property

    Public Property Name As String Implements IConsoleEntity.Name
        Get
            Return MyName
        End Get
        Set(value As String)
            MyName = value
        End Set
    End Property

    Public Sub New(Name As String, Location As Integer, Population As Decimal, Capacity As Decimal, Resources As Byte, Colour As ConsoleColor, Art As String)
        Me.Name = Name
        Me.Location = Location
        Me.Population = Population
        Me.Capacity = Capacity
        Me.Resources = Resources
        Me.Colour = Colour
        Me.Art = Art
    End Sub

    Public Sub Draw() Implements IConsoleEntity.Draw

        Console.ForegroundColor = Me.Colour
        Console.Write(Me.Art)
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine(vbTab & "{0} {1}pc", Me.Name, Me.Location)

    End Sub
End Class
