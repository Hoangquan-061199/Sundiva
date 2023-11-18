namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlHistoryDeposit
    {
        public const string SqlByListCustomer = "SELECT * FROM HistoryDeposit WHERE CustomerID = {0} {1} ";
    }
}
