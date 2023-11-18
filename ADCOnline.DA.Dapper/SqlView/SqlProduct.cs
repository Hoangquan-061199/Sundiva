namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlProduct
    {
        //SQL for Customer Object
        public const string SqlView = "SELECT top {0}  *,'{1}' PositionIds,','+ModuleIds+',' ModuleViewIds," +
            "(select b.Icon from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) LogoBrand," +
            "(select b.Name from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) NameBrand" +
            " FROM Product WHERE ID in(" +
                                      " SELECT ID FROM Product WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' {3})" + " {4}";
        public const string SqlHome = " SELECT ID,Name,NameAscii,LinkUrl,UrlPicture,PositionCode,ParentID,Description, TypeView,ModuleIds,DataJson,Video,AlbumPictureJson,ViewHome,SeoDescription,SeoKeyword,SEOTitle,TotalViews,CreatedDate FROM(" +
             "SELECT top {0}  ID,Name,NameAscii,LinkUrl,UrlPicture,'{1}' PositionCode,null ParentID,Description, null TypeView, ','+ModuleIds+',' ModuleIds,DataJson,Video,AlbumPictureJson,ViewHome,SeoDescription,SeoKeyword,SEOTitle,TotalViews,CreatedDate  FROM Product WHERE ID in(" +
                                      " SELECT ID FROM Product WHERE IsDeleted =0 AND IsShow =1 {2})" + " {3}) c";
        public const string SqlByNamAscci = "SELECT c.*, m.Name ModuleName, m.NameAscii ModuleNameAscii,m.ModuleTypeCode, m.ID ModuleId FROM Product c, WebsiteModule m WHERE c.IsDeleted =0 AND c.IsShow =1 AND m.IsDeleted =0 AND m.IsShow =1 " +
                                                  " AND ','+c.ModuleIds+',' like '%,'+convert(varchar,m.ID)+',%' AND c.NameAscii = '{0}' {1}";
        public const string SqlByModuleNamAscci = "SELECT c.*, m.Name ModuleName, m.NameAscii ModuleNameAscii FROM Product c, WebsiteModule m WHERE c.IsDeleted =0 AND c.IsShow =1 AND m.IsDeleted =0 AND m.IsShow =1 " +
                                                  " AND ','+c.ModuleIds+',' like '%,'+convert(varchar,m.ID)+',%' AND m.NameAscii = '{0}' {1}";
        public const string SqlByModuleId = "SELECT * FROM Product WHERE IsDeleted =0 AND IsShow =1 AND ','+ModuleIds+',' like '%,{0},%' {1}";
        public const string SqlByListModuleId = "SELECT top {0}  *  FROM Product WHERE ID in(" +
                                      " SELECT ID FROM Product WHERE IsDeleted =0 AND IsShow =1 AND IsApproved=1 {1})" + " {2}";
        public const string SqlByListPageModuleId = "SELECT  *  FROM Product WHERE ID in(" +
                                    " SELECT ID FROM Product WHERE IsDeleted =0 AND IsShow =1 AND Lang = '{0}' {0})" + " {1}";
        public const string SqlBySearch = "SELECT * {0}  FROM Product  WHERE IsDeleted =0 AND IsShow =1 {1}";
        public const string SqlByManager = "SELECT *  FROM Product  WHERE IsDeleted =0 AND IsShow =1 {0}";
        public const string SqlByCustomer = "SELECT *  FROM Product  WHERE IsDeleted =0 {0}";
        public const string SqlId = "SELECT * FROM Product WHERE IsDeleted =0 AND IsShow =1 AND ID IN({0}) {1}";
    }
}
