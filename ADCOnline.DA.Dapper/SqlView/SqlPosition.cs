namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlPosition
    {
        //SQL for Customer Object
        public const string Sql = "SELECT * FROM ModulePosition WHERE ParentId = (SELECT ID FROM ModulePosition WHERE Code='{0}') OR Code='{0}'";
        public const string SqlHome = "with name_tree as (" +
            " select *,OrderTree =0 from ModulePosition where Code = '{0}' union all" +
            " select c.*,OrderTree=OrderTree+1 from ModulePosition c join name_tree p on p.ID = c.ParentId  AND c.IsDeleted=0 And c.IsShow = 1)" +
            " select * from name_tree ORDER BY OrderTree ASC";
    }
}
