namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlHistoryTransaction
    {
        public const string SqlByListCustomer = "SELECT * FROM HistoryTransaction WHERE CustomerID = {0} {1} ";
    }
}
