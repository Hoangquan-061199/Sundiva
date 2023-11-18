namespace ADCOnline.DA.Dapper.SqlView
{
    public static class SqlModule
    {
        //SQL for Customer Object
        public const string SqlHome = " SELECT * FROM(" +
             "SELECT top {0} ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,','+PositionCode+',' PositionCode,ParentID,Description,'{1}' TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode,Content FROM WebsiteModule " +
             "WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' AND ','+PositionCode+',' like '%,{3},%' {4}) c";
        public const string SqlHomeChild =
            " SELECT ID,Name,NameAscii,LinkUrl,UrlPicture,PositionCode,ParentID,Description,TypeView,Type Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay FROM(" +
            "SELECT top {0} ID,Name,NameAscii,LinkUrl,UrlPicture,','+PositionCode+',' PositionCode, ParentID,Description,'{1}' TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode FROM WebsiteModule " +
            "WHERE IsDeleted =0 AND IsShow =1  AND (','+PositionCode+',' like '%,{2},%') " +
            "OR (ParentID in(SELECT ID FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionCode+',' like '%,{2},%')) {3})c";

        public const string SqlHomeGrandchildren = " SELECT ID,Name,NameAscii,LinkUrl,UrlPicture,PositionCode,ParentID,Description,TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel FROM(" +
            "SELECT top {0} ID,Name,NameAscii,LinkUrl,UrlPicture,','+PositionCode+',' PositionCode, ParentID,Description,'{1}' TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode FROM WebsiteModule " +
            "WHERE IsDeleted =0 AND IsShow =1  AND ((','+PositionCode+',' like '%,{2},%') " +
            "OR (ParentID in(SELECT ID FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionCode+',' like '%,{2},%'))" +
            "OR ParentID in(SELECT ID FROM WebsiteModule WHERE ParentID in(SELECT ID FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionCode+',' like '%,{2},%'))) {3})c";

        public const string SqlByChildNamAscci = "SELECT * FROM WebsiteModule WHERE ParentID in(SELECT ID FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND NameAscii = '{0}' {1}) " +
                                                 " OR ID in(SELECT ID FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND NameAscii = '{0}' {1}) {2} ";
        public const string SqlByNamAscci = "SELECT * FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND NameAscii = '{0}' {1} ";
        public const string SqlByParentID = "SELECT * FROM WebsiteModule WHERE IsDeleted =0 AND IsShow =1 AND ParentID = '{0}' {1} ";

        public const string SqlGetParentbyChild = "with name_tree as(select id, parentid,Name,NameAscii,ModuleTypeCode,OrderTree =0" +
            " from WebsiteModule where id = {0} union all"+
            " select c.id, c.parentid,c.Name,c.NameAscii,c.ModuleTypeCode,OrderTree=OrderTree+1 from WebsiteModule c" +
            " join name_tree p on C.id = P.parentid  AND (C.id<>C.parentid OR c.ParentID is null))" +
            " select * from name_tree OPTION (MAXRECURSION 0)";

        public const string SqlGetChildByParent = "with name_tree as (select id, parentid,Name,NameAscii,ModuleTypeCode,Rel,OrderTree =0" +
                                                  " from WebsiteModule where id = {0} union all" +
                                                  " select c.id, c.parentid,c.Name,c.NameAscii,c.ModuleTypeCode,Rel,OrderDisplay=OrderTree+1 from WebsiteModule c" +
                                                  " join name_tree p on p.id = c.parentid  AND c.IsDeleted=0)" +
                                                  " select * from name_tree";
        public const string SqlGetParentbyChildNameAscci = "with name_tree as(select id, parentid,Name,NameAscii,ModuleTypeCode,OrderTree =0" +
                                                  " from WebsiteModule where NameAscii = '{0}' AND IsDeleted =0 union all" +
                                                  " select c.id, c.parentid,c.Name,c.NameAscii,c.ModuleTypeCode,Rel,OrderTree=OrderTree+1 from WebsiteModule c" +
                                                  " join name_tree p on C.id = P.parentid AND IsDeleted =0 AND (C.id<>C.parentid OR c.ParentID is null)) " +
                                                  " select * from name_tree OPTION (MAXRECURSION 0)";

        public const string SqlGetChildByParentNameAscci = "with name_tree as (select id, parentid,Name,NameAscii,ModuleTypeCode,Rel,OrderTree =0" +
                                                  " from WebsiteModule where NameAscii = '{0}' AND IsDeleted =0 union all" +
                                                  " select c.id, c.parentid,c.Name,c.NameAscii,c.ModuleTypeCode,OrderDisplay=OrderTree+1 from WebsiteModule c" +
                                                  " join name_tree p on p.id = c.parentid AND IsDeleted =0)" +
                                                  " select * from name_tree";
        public const string SqlGetParentNull = "select id, parentid,Name,NameAscii,ModuleTypeCode,Rel, 1 OrderTree FROM WebsiteModule WHERE ParentID is null AND ModuleTypeCode ='Product' AND IsDeleted =0  AND IsShow =1";

        public const string SqlAdmin = " SELECT * FROM(" +
             "SELECT top {0} ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,ParentID,Description,'{1}' TypeView,Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay, ModuleTypeCode FROM WebsiteModule " +
             "WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' AND ','+PositionCode+',' like '%,{3},%' {4}) c";
        public const string SqlAdminIds = " SELECT * FROM(" +
             "SELECT top {0} ID,Name,Title,NameAscii,LinkUrl,UrlPicture,'' UrlPictureMobile,Video,ParentID,Description,'{1}' TypeView,Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay, ModuleTypeCode FROM WebsiteModule " +
             "WHERE IsDeleted =0 AND IsShow =1 And Lang = '{2}' AND ','+PositionIds+',' like '%,{3},%' {4}) c";
    }
}
