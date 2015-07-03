Public Structure SystemTotals

    Public PopulationChange As Decimal
    Public CashIncome As Decimal
    Public TechIncome As Decimal
    Public ProductionIncome As Decimal

    Public Population As Decimal
    Public Capacity As Decimal

    Public Sub New(Pop As Decimal, Cap As Decimal, CashIncome As Decimal, TechIncome As Decimal, ProductionIncome As Decimal) ', Population As Decimal, Production As Decimal)
        Me.Population = Pop
        Me.Capacity = Cap
        Me.CashIncome = CashIncome
        Me.TechIncome = TechIncome
        Me.ProductionIncome = ProductionIncome
    End Sub

End Structure
