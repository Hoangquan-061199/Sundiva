using System;
using System.Collections.Generic;
namespace ADCOnline.Utils
{
    public class StaticEnum
    {
        #region Layout
        public const string HeadTop = "HeadTop";//HeadTop
        public const string Logo = "Logo";
        public const string MainMenu = "MainMenu";
        public const string Menu2 = "Menu2";
        public const string MainTop = "MainTop";
        public const string AboutIndex = "AboutIndex";
        public const string PartnerIndex = "PartnerIndex";
        public const string CustomersIndex = "CustomersIndex";
        public const string Partner = "Partner";
        public const string ServiceIndex = "ServiceIndex";
        public const string RightMenu = "RightMenu";
        public const string VideoIndex = "VideoIndex";
        public const string NewsHot = "NewsHot";
        public const string LayoutHome = "LayoutHome";//Index
        public const string Popup = "Popup";
        public const string SlideHome = "SlideHome";
        public const string TourIndex = "TourIndex";
        public const string LastMinuteTourDeals = "LastMinuteTourDeals";
        public const string TourInCountry = "TourInCountry";
        public const string TourOutCountry = "TourOutCountry";
        public const string NewsIndex = "NewsIndex";
        public const string ApplicationWorks = "ApplicationWorks";
        public const string Business = "Business";
        public const string ProjectHot = "ProjectHot";
        public const string CompanyActivities = "CompanyActivities";
        public const string ProductModuleLeft = "ProductModuleLeft";
        public const string ADSProductModuleLeft = "ADSProductModuleLeft";
        public const string ProductNotModuleLeft = "ProductNotModuleLeft";
        public const string ADSProductNotModuleLeft = "ADSProductNotModuleLeft";
        public const string ADSTeckChem = "ADSTeckChem";
        public const string ModuleProduct = "ModuleProduct";
        public const string ProductIndex = "ProductIndex";
        public const string KohlerA800 = "KohlerA800";
        public const string KohlerLift = "KohlerLift";
        public const string KohlerEV1 = "KohlerEV1";
        public const string Worldwide = "Worldwide";
        public const string Application = "Application";
        public const string ProductHot = "ProductHot";
        public const string FreeInformationPack = "FreeInformationPack";
        public const string KhuyenMai = "KhuyenMai";
        public const string BannerPhai = "BannerPhai";
        public const string TinTucNoiBat = "TinTucNoiBat";
        public const string TechnicalAdviceIndex = "TechnicalAdviceIndex";
        public const string Footer = "Footer";//Footer
        public const string MenuFooterColumn = "MenuFooterColumn";
        public const string MenuFooterColumn1 = "MenuFooterColumn1";
        public const string GoogleMap = "GoogleMap";
        public const string LogoFooter = "LogoFooter";
        public const string FormContact = "FormContact";
        public const string MenuFooterRow = "MenuFooterRow";
        public const string RightLayout = "RightLayout";//RightLayout
        public const string ModuleGuide = "ModuleGuide";
        public const string SpecialistIndex = "SpecialistIndex";
        public const string DoctorTeamIndex = "DoctorTeamIndex";
        public const string RegisterIndex = "RegisterIndex";
        public const string ContactIndex = "ContactIndex";
        public const string BrandIndex = "BrandIndex";
        public const string CustomerIndex = "CustomerIndex";
        #endregion
        #region Kiểu hiển thị
        public const string ContactUs = "ContactUs";
        public const string CustomerGuide = "CustomerGuide";
        public const string Doctorteam = "Doctorteam";
        public const string Specialist = "Specialist";
        public const string News = "News";
        public const string BusinessAreas = "BusinessAreas";
        public const string Gallery = "Gallery";
        public const string QA = "QA";
        public const string Updating = "Updating";
        public const string Store = "Store";
        public const string Product = "Product";
        public const string Manufacturer = "Manufacturer";
        public const string Project = "Project";
        public const string Introduce = "Introduce";
        public const string ContentIntroduce = "ContentIntroduce";
        public const string ContentAccountInfor = "ContentAccountInfor";
        public const string AboutUs = "AboutUs";
        public const string Contact = "Contact";
        public const string SimpleModule = "SimpleModule";
        public const string Landing = "Landing";
        public const string LandingPage = "LandingPage";
        public const string Document = "Document";
        public const string Company = "Company";
        public const string Recuitment = "Recuitment";
        public const string Services = "Services";
        public const string Sale = "Sale";
        public const string Album = "Album";
        public const string Video = "Video";
        public const string Trademark = "Trademark";
        public const string Cart = "Cart";
        public const string LogIn = "LogIn";
        public const string Register = "Register";
        public const string History = "History";
        public const string Payment = "Payment";
        public const string Build = "Build";
        public const string Library = "Library";
        public const string DistributionSystem = "DistributionSystem";
        public const string KeyCapcha = "KeyCapcha";
        public const string Search = "Search";
        #endregion
        #region Thông tin fix
        public static string UnitDefault = "đ";
        #endregion
        #region Quyền
        public static string RoleALL = "ALL"; //có thể sử dụng được tất cả
        public static string RoleAdmin = "Admin"; // admin
        public static string RoleStaff = "Staff"; // nhân viên
        #endregion
        #region Loại quảng cáo

        public const string AdvImg = "1";
        public const string AdvHtml = "2";

        #endregion
        #region loại hiển thị giới thiệu
        public const string ThuNgo = "ThuNgo";
        public const string Quatrinh = "Quatrinh";
        public const string TamNhin = "TamNhin";
        public const string SoDo = "SoDo";
        #endregion
        #region loại hiển thị thư viện
        public const string Videos = "Videos";
        public const string Picture = "Picture";
        public const string HoatDongXaHoi = "HoatDongXaHoi";
        public const string HoatDongTapThe = "HoatDongTapThe";
        public const string TaiLieuInox = "TaiLieuInox";
        #endregion
        #region loại hiển thị đối tác

        public const string CongTrinhUngDung = "CongTrinhUngDung";
        public const string KhachHangDoiTac = "KhachHangDoiTac";
        public const string TrachNhiemXaHoi = "TrachNhiemXaHoi";
        public const string CoiTrongNguoiLaoDong = "CoiTrongNguoiLaoDong";
        public const string KhachHangTrungTam = "KhachHangTrungTam";
        #endregion
        #region Thông tin ViewHome

        //thông tin ViewHome hết số 9 thì chuyển thành A không được chuyển số

        // 2: IsHightLight : View nổi bật

        public const int IsNew = 0;// 0:IsHome : mới
        public const int IsHome = 1;// 1:IsHome : view trang chủ
        public const int IsSelling = 2; // bán chạy
        public const int IsHot = 3;// 3: IsHot : tin hot
        public const int IsBestSell = 4; //giá gốc
        public const int IsBestSale = 5;

        #endregion
        #region Loại hiển thị
        //loại hiển thị
        public static string Advertising = "Advertising";
        public static string Module = "Module";
        public static string Content = "Content";
        #endregion       
        #region Mã Langding
        #endregion
        #region Code nội dung khác
        public const string WhyChooseRight = "WhyChooseRight";
        public const string ContactFooer = "ContactFooer";
        public const string ContentContact = "ContentContact";
        public const string TermsofUse = "TermsofUse";
        #endregion        
        #region Status Order
        public const int Paid = 0; // đã thanh toán
        public const int New = 1; // đơn mới
        public const int Processing = 2; // đang xử lý
        public const int Sending = 3; // đang gửi
        public const int Finshed = 4; // kết thúc
        public const int Return = 5; // trả lại
        public const int Spam = 6; // spam
        public const int Remind = 7; // nhắc nhở
        #endregion
        #region Hàm cố định trong admin
        public static string Status(DateTime? endDate, int? status, bool? approve)
        {
            if (endDate != null)
            {
                double mutine = (endDate - DateTime.Now).Value.TotalMinutes;
                if (mutine < 0)
                {
                    return "<span class='label label-danger'>Đã hết hạn</span>";
                }
            }
            status = ConvertUtil.ToInt32(status);
            if (status == 1)
            {
                return approve == true ? "Đang hoạt động" : "Đang chờ duyệt";
            }
            return status == 2 ? "Đã từ chối" : "Đã tạm ngừng";
        }
        public static string StatusServices(DateTime? endDate, string status)
        {
            if (endDate != null)
            {
                double mutine = (endDate - DateTime.Now).Value.TotalMinutes;
                if (mutine < 0)
                {
                    return "<span class='badge badge-danger'>Đã tạm dừng</span>";
                }
            }
            switch (status)
            {
                case "ACTIVE":
                    {
                        return "<span class='badge badge-success'>Đang hoạt động</span>";
                    }
                case "PENDING":
                    {
                        return "<span class='badge badge-warning'>Đang chờ duyệt</span>";
                    }
                case "STOP":
                default:
                    {
                        return "<span class='badge badge-danger'>Đã tạm dừng</span>";
                    }
            }
        }
        public static string Show(bool? status) => status == true ? "<span class='badge badge-success'>Hiện</span>" : "<span class='badge badge-danger'>Ẩn</span>";
        public static string Sitemap(bool? status) => status == true
                ? "<span class='badge badge-success'>sitemap</span>"
                : "<span class='badge badge-danger'>sitemap</span>";
        public static string VAT(bool? status) => status == true ? "<span class='badge badge-success'>Đã có VAT</span>" : "<span class='badge badge-danger'>Chưa có VAT</span>";
        public static string MultilOrder(bool? status) => status == true ? "<span class='badge badge-success'>Có</span>" : "<span class='badge badge-danger'>Không</span>";
        public static string IsProductHot(string viewhome) => ("," + viewhome + ",").Contains(",3,")
                ? "<img src=\"/Admin/images/icon-hot.png\" width=\"33\" height=\"25\" data-toggle=\"tooltip\" data-placement=\"bottom\" style=\"float:right;\" title=\"Sản phẩm nổi bật\">"
                : string.Empty;
        public static string IsProductBestSale(string viewhome)
        => ("," + viewhome + ",").Contains(",5,") ? "<span style=\"float:right;margin-right:5px;margin-top:5px;\" class=\"badge badge-warning\">Giá sốc</span>" : string.Empty;
        public static string Selling(string viewhome)
        => ("," + viewhome + ",").Contains(",2,") ? "<img src=\"/Admin/images/best-seller.svg\" width=\"25\" height=\"25\" style=\"float:right;\" data-toggle=\"tooltip\" data-placement=\"bottom\" title=\"Sản phẩm bán chạy\">" : string.Empty;
        public static string Approved(bool? status)
        => status == true ? "<span class='badge badge-success'>Đã duyệt</span>" : "<span class='badge badge-danger'>Chưa duyệt</span>";
        #endregion
        #region Data Json Code
        public const string ReceiverEmail = "ReceiverEmail";
        #endregion
        #region Type module product
        public const string TypeProduct = "TypeProduct";
        public const string TypeProductI = "TypeProductI";
        public const string TypeProductII = "TypeProductII";
        public const string TypeProductIII = "TypeProductIII";
        #endregion

        #region Type menu
        public const string MenuI = "MenuI";
        public const string MenuII = "MenuII";
        public const string MenuIII = "MenuIII";
        #endregion
        #region Type Notification
        public const string NotiSale = "NotiSale";
        public const string NotiOrder = "NotiOrder";
        public const string NotiSystem = "NotiSystem";
        #endregion
        public const string DefaultLanguage = Resources.VI;
        private static readonly List<string> list = new()
        {
            News,Contact,Company,Project,Services,Gallery,Video,Introduce,Document,Updating,Recuitment,
            Library,Build,Application,ContactUs,
            CompanyActivities,BusinessAreas,Video
        };
        public static List<string> ModuleContent = list;
        private static readonly List<string> listSitemap = new()
        {  News,Contact,Company,Project,Services,Gallery,Video,Introduce,Document,Updating,Recuitment,Library,Build,History,Application
        };
        public static List<string> ModuleSitemap = listSitemap;
    }
}
