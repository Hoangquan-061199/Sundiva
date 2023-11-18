namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlOrder
    {
        public const string SqlByBuySeller = "SELECT * FROM (SELECT o.*, p.PaymentType ProductPaymentType,p.CustomerDataJson CustomerProduct, p.NameAscii ProductNameAscii,p.ModuleNameAscii ProductModuleNameAscii, p.IsAdv ProductIsAdv, p.Name ProductName FROM [Order] o"
            + " LEFT JOIN Product p on p.ID = o.ProductID WHERE o.IsDeleted =0 AND (p.CustomerID ={0} OR o.CustomerID={0}) {1}) c  Order by CreatedDate desc";
    }
}
