namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlTags
    {
        //SQL for Customer Object
        public const string SqlByContentId = "SELECT * FROM SystemTag WHERE IsDeleted =0 AND IsShow =1 {0}";
        public const string SqlNameAscii = "SELECT * FROM SystemTag WHERE IsDeleted =0 AND IsShow =1 AND NameAscii = '{0}' {1}";
        public const string SqlById = "SELECT * FROM SystemTag WHERE IsDeleted =0 AND IsShow =1 AND ID in({0})";
    }
}
