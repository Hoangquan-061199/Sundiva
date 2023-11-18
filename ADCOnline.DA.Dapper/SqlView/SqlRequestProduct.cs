namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlRequestProduct
    {
        public const string SqlByListCustomer = "SELECT * FROM RequestProduct WHERE CustomerID = {0} {1} ";
        public const string SqlByList = "SELECT * FROM RequestProduct WHERE {0} ";
    }
}
