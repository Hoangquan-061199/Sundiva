using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ADCOnline.Utils
{
    //Dùng để chứa các hàm sau này có thể xóa khi sang project khác.
    //VD: trạng thái đơn hàng.
    public static class Temp
    {
        public static string PositionLogo(string type)
        {
            return type switch
            {
                "BottomCenter" => "bottom-center",
                "BottomRight" => "bottom-right",
                "BottomLeft" => "bottom-left",
                "TopCenter" => "top-center",
                "TopRight" => "top-right",
                "TopLeft" => "top-left",
                "Center" => "center",
                _ => string.Empty,
            };
        }
        public static string OrderStatus(int? type)
        {
            return type switch
            {
                6 => "Đã hủy",
                5 => "Đã hoàn thành",
                3 => "Đang giao",
                2 => "Chờ lấy hàng",
                1 => "Chờ xử lý",
                _ => "Đơn hàng mới",
            };
        }
        public static string TypeCampain(int? type)
        {
            return type switch
            {
                2 => "Giảm % đơn hàng trong khoảng giá tất cả sản phẩm",
                3 => "Giảm số tiền cố định trong khoảng giá tất cả sản phẩm",
                6 => "Giảm % cho các sản phẩm trong module hoặc sản phẩm cụ thể",
                7 => "Giảm số tiền cố định cho các sản phẩm trong module hoặc sản phẩm cụ thể",
                _ => "N/a",
            };
        }
        public static string Gender(int? type)
        {
            return type switch
            {
                1 => "Nam",
                0 => "Nữ",
                2 => "Khác",
                _ => "Nam",
            };
        }
        public static string TextTypeMenu(string type)
        {
            return type switch
            {
                "TypeProduct" => "Du lịch",
                "TypeProductI" => "Thuê xe",
                "TypeProductII" => "Khách sạn",
                "TypeProductIII" => "Khác II",
                "MenuI" => "Trong nước",
                "MenuII" => "Nước ngoài",
                "MenuIII" => "Khách đoàn",
                _ => "",
            };
        }
        public static string TextSort(string type)
        {
            return type switch
            {
                "1" => "Sản phẩm mới nhất",
                "2" => "Từ giá thấp đến cao",
                "3" => "Từ giá cao đến thấp",
                _ => "",
            };
        }
        public static string TextNotificationType(string type)
        {
            return type switch
            {
                StaticEnum.NotiSale => "Khuyến mãi",
                StaticEnum.NotiOrder => "Đơn hàng",
                StaticEnum.NotiSystem => "Hệ thống",
                _ => "Chưa chọn",
            };
        }
        public static string ImgNotificationType(string type)
        {
            return type switch
            {
                StaticEnum.NotiSale => "icon-noti-sale.png",
                StaticEnum.NotiOrder => "icon-noti-order.png",
                StaticEnum.NotiSystem => "icon-noti-system.png",
                _ => "icon-noti-sale.png",
            };
        }
        public static string ClassNotificationType(string type)
        {
            return type switch
            {
                StaticEnum.NotiSale => "badge-warning",
                StaticEnum.NotiOrder => "badge-info",
                StaticEnum.NotiSystem => "badge-danger",
                _ => "badge-success",
            };
        }
        public static string TextGender(string type)
        {
            return type switch
            {
                "0" => "Nam",
                "1" => "Nữ",
                "2" => "Khác",
                _ => "Nam",
            };
        }
        public static string TypeContact(string type)
        {
            return type switch
            {
                "Contact" => "Liên hệ",
                "BookingDate" => "Đặt lịch hẹn",
                "Advisor" => "Tư vấn",
                "Distributor" => "Trờ thành nhà phân phối",
                "QuickBuy" => "Đặt mua ngay",
                _ => "",
            };
        }
        public static string PaymentTypeText(string type)
        {
            return type switch
            {
                "6" => "Đặt cọc chuyển khoản",
                "5" => "Thanh toán bằng QR Code",
                "4" => "Cà thẻ khi nhận hàng",
                "3" => "Thanh toán online bằng thẻ VISA, MasterCard",
                "2" => "Thanh toán trực tuyến bằng thẻ ATM nội địa đã được đăng ký Internet Banking. ",
                "1" => "Thanh toán tiền mặt khi nhận hàng",
                _ => "Thanh toán tiền mặt khi nhận hàng (COD)",
            };
        }
        public static string OrderStatusCss(int? type)
        {
            return type switch
            {
                10 => "badge-danger",
                9 => "badge-warning",
                8 => "badge-warning",
                7 => "badge-info",
                6 => "",
                5 => "badge-success",
                4 => "ovrbil",
                3 => "offbil",
                1 => "badge-warning newbil",
                _ => "badge-warning",
            };
        }
        public static string OrderStatusTrCss(int? type)
        {
            return type switch
            {
                4 or 3 or 2 or 1 => "badge-warning newbil",
                _ => "",
            };
        }
        public static string OrderStatusCssMember(int? type)
        {
            return type switch
            {
                6 => "cancel-order",
                5 => "success-order",
                3 or 2 or 1 => "suppend-order",
                _ => "new-order",
            };
        }
        public static string SizeImage(string type)
        {
            return type switch
            {
                "Banner" => "1366x358",
                "AvatarProduct" => "800x800",
                "AvatarProject" => "618x380",
                "AvatarServices" => "595x309",
                "AvatarNews" => "386x238",
                _ => string.Empty
            };
        }

        //public static string TextIntro(string type)
        //{
        //    return type switch
        //    {
        //        StaticEnum.GioiThieuChung => "Giới thiệu chung",
        //        StaticEnum.HoSoNangLuc => "Hồ sơ năng lực",
        //        _ => "Chứng chỉ CO-CQ",
        //    };
        //}
    }
}
