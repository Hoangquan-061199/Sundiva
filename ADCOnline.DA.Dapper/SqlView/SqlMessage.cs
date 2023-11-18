namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlMessage
    {
        //SQL for Customer Object
        public const string SqlBySearch = "SELECT * {0}  FROM Message  WHERE IsDeleted =0 {1}";
        public const string SqlByPage = "SELECT *  FROM Message  WHERE IsDeleted =0 {0}";
        public const string SqlByPageCustomerId = "SELECT *, CASE WHEN ','+CustomerRead+',' like '%,{0},%' THEN 1 ELSE 0  END AS IsRead FROM Message  WHERE IsDeleted =0 AND (SenderID ={0} OR ',' + ReciverIDs + ',' like '%,{0},%') {1} ORDER BY IsRead asc, CreatedDate desc";
        public const string SqlByNotification = "SELECT m.* FROM Message m" +
                                                " LEFT JOIN Customer c on m.ReciverIDs = CONVERT(nvarchar,c.ID)" +
                                                " WHERE m.IsDeleted =0 AND Type >=1 AND ISNULL(EndDate,GETDATE()) >= GETDATE()" +
                                                " AND (ReciverIDs is null OR ',' +ReciverIDs+',' like '%,{0},%') AND c.CreatedOnUtc <= m.CreatedDate) c" +
                                                " Order by CreatedDate DESC";
        public const string SqlByMessage = "SELECT * FROM Message  WHERE IsDeleted =0 AND Type =0 AND ISNULL(EndDate,GETDATE()) >= GETDATE()" +
                                                " AND (ReciverIDs is null OR ',' +ReciverIDs+',' like '%,{0},%') Order by CreatedDate DESC";
        public const string SqlByPageCustomerIdSend = "SELECT * FROM Message  WHERE IsDeleted =0 AND SenderID ={0} {1} ORDER BY CreatedDate desc";
        public const string SqlByOrderBuy = "select * FROM(SELECT m.* FROM Message m INNER JOIN [Order] o on o.ID =m.OrderId AND o.CustomerID ={0} ) c WHERE  ',' + c.ReciverIDs + ',' like '%,{0},%' {1} ORDER BY CreatedDate desc";
        public const string SqlByOrderSell = "select * FROM(SELECT m.* FROM Message m INNER JOIN [Order] o on o.ID =m.OrderId AND o.SellerID ={0} )  c WHERE  ',' + c.ReciverIDs + ',' like '%,{0},%' {1}  ORDER BY CreatedDate desc";
        public const string SqlByDetailOrderBuy = "SELECT m.*, CASE WHEN ','+CustomerRead+',' like '%,{0},%' THEN 1 ELSE 0  END AS IsRead FROM Message m INNER JOIN [Order] o on o.ID =m.OrderId AND o.CustomerID ={0}  WHERE m.IsDeleted =0 AND m.ID={1} ORDER BY IsRead asc, m.CreatedDate desc";

        public const string SqlDetail = "SELECT * FROM Message  WHERE IsDeleted =0 AND ((SenderID ={0} AND ',' + ReciverIDs + ',' like '%,{1},%') OR ( SenderID ={1} AND ',' + ReciverIDs + ',' like '%,{0},%')) AND Type in({2})  ORDER BY CreatedDate desc";
        public const string SqlDetailById = "SELECT * FROM Message  WHERE IsDeleted =0 AND ID={0}  ORDER BY CreatedDate desc";
        public const string SqlByUnMessage = "SELECT * FROM Message  WHERE IsDeleted =0 AND (SenderID ={0} OR ',' + ReciverIDs + ',' like '%,{0},%') ORDER BY CreatedDate desc";
        public const string SqlByPageNotificationCustomerId = "SELECT * FROM(SELECT m.* FROM Message m LEFT JOIN Customer c on m.ReciverIDs = CONVERT(nvarchar,c.ID)"+
			" WHERE m.IsDeleted =0 AND Type >=1 AND ISNULL(EndDate,GETDATE()) >= GETDATE()"+
			" AND c.CreatedOnUtc <= m.CreatedDate AND  (ReciverIDs is null OR ReciverIDs = Convert(varchar(11),{0}))"+
			" ) m Order by m.UpdatedDate DESC";

        public const string sqlUnread =
            "SELECT * FROM Message WHERE IsDeleted =0 AND ISNULL(Type,0) =0 AND (SenderID ={0} OR ',' + ReciverIDs + ',' like '%,{0},%') " +
            " AND ','+ISNULL(CustomerRead,'')+','  NOT LIKE '%,'+Convert(varchar(11),{0})+',%' ORDER BY UpdatedDate DESC,CreatedDate DESC";

        public const string sqlAllMessage =
            "SELECT * FROM Message WHERE IsDeleted =0 AND ISNULL(Type,0) =0 AND (SenderID ={0} OR ',' + ReciverIDs + ',' like '%,{0},%') " +
            " ORDER BY UpdatedDate DESC,CreatedDate DESC";
    }
}
