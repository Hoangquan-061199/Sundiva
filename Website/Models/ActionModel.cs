using Newtonsoft.Json;
using System;

namespace Website.Models
{
    public class ActionViewModel
    {
        [JsonProperty("do")]
        public string Do { get; set; }
        public string ItemId { get; set; }
        public string ParentId { get; set; }
        public string ModuleId { get; set; }
        public string Status { get; set; }
        public string Keys { get; set; }
    }

    public class ActionType
    {
        public const string Add = "Add"; //thêm

        public const string Edit = "Edit"; //sửa
        public const string Show = "Show"; //hiển thị
        public const string Delete = "Delete"; //Xóa
        public const string DeleteAll = "DeleteAll"; //Xóa
        public const string Hidden = "Hidden"; //Ẩn
        public const string Display = "Display"; //Hiển thị
        public const string ShowAll = "ShowAll"; //Ẩn chọn
        public const string HiddenAll = "HiddenAll"; //Ẩn chọn
        public const string OrderBy = "OrderBy"; //Ẩn chọn
        public const string Role = "Role"; //phan quyen
        public const string Active = "Active"; //duyệt
        public const string EmbassyAll = "EmbassyAll";
        public const string DisEmbassyAll = "DisEmbassyAll";
        public const string RegulationsAll = "RegulationsAll";
        public const string DisRegulationsAll = "DisRegulationsAll";
        public const string Approved = "Approved";
        public const string NotApproved = "NotApproved";
        public const string ApprovedAll = "ApprovedAll";
        public const string BestSell = "BestSell";
        public const string NotBestSell = "NotBestSell";
        public const string NotApprovedAll = "NotApprovedAll";
        public const string ChangePrice = "ChangePrice";
        public const string ChangePriceOld = "ChangePriceOld";
        public const string ChangeSale = "ChangeSale";
        public const string ChangeSource = "ChangeSource";
        public const string IsNew = "IsNew";
        public const string NotNew = "NotNew";
        public const string IsHot = "IsHot";
        public const string NotHot = "NotHot";
        public const string IsHome = "IsHome";
        public const string NotHome = "NotHome";
        public const string IsShock = "IsShock";
        public const string NotShock = "NotShock";
        public const string IsSelling = "IsSelling";
        public const string NotIsSelling = "NotIsSelling";
        public const string ChangeStatus = "ChangeStatus";
        public const string ChangeStatusAll = "ChangeStatusAll";
        public const string UpdateSale = "UpdateSale";
        public const string AddFrames = "AddFrames";
        public const string RemoveFrames = "RemoveFrames";
        public const string UpdateAmount = "UpdateAmount";
        public const string ChangeViewHome = "ChangeViewHome";
        public const string AddModule = "AddModule";
        public const string RemoveModule = "RemoveModule";
        public const string AddQuick = "AddQuick";
        public const string IsVat = "IsVat";
        public const string IsVatAll = "IsVatAll";
        public const string NotIsVatALL = "NotIsVatALL";
        public const string IsSitemap = "IsSitemap";
        public const string IsSitemapAll = "IsSitemapAll";
        public const string NotIsSitemapAll = "NotIsSitemapAll";
        public const string Feature = "Feature";
        public static string ActionText(string action)
        {
            switch (action)
            {
                case Edit:
                    return "Sửa";
                case Show:
                    return "Hiển thị";
                case UpdateSale:
                case UpdateAmount:
                case AddModule:
                case RemoveModule:
                    return "Cập nhật";
                default:
                    return "Thêm mới";
            }
        }
    }

   
}