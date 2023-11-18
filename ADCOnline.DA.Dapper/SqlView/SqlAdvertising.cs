namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlAdvertising
    {
        //SQL for Customer Object
        public const string SqlHome = " SELECT * FROM(" +
          "SELECT TOP {0} ID,Name,'' Title,null AS NameAscii,LinkUrl,UrlPicture,Video,','+PositionCode+',' PositionCode,ParentID,Description, '{1}' TypeView,CONVERT(nvarchar(500),Type) Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,null Rel, null ModuleTypecode,Content FROM Advertising " +
          "WHERE IsDeleted =0 AND IsShow =1 And Lang ='{2}' AND ','+PositionCode+',' like '%,{3},%' {4} )c";
        public const string SqlAdmin = " SELECT * FROM(" +
         "SELECT top {0} ID,Name,'' Title,null AS NameAscii,LinkUrl,UrlPicture,Video,ParentID,Description,'{1}' TypeView,CONVERT(nvarchar(500),Type) Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,null ModuleTypeCode FROM Advertising " +
         "WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' AND ','+PositionCode+',' like '%,{3},%' {4}) c";
        public const string SqlAdminIds = " SELECT * FROM(" +
         "SELECT top {0} ID,Name,'' Title,null AS NameAscii,LinkUrl,UrlPicture,UrlPictureMobile,Video,ParentID,Description,'{1}' TypeView,CONVERT(nvarchar(500),Type) Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,null ModuleTypeCode FROM Advertising " +
         "WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' AND ',' + PositionIds +',' like '%,{3},%' {4}) c";
    }
}
