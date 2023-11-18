using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class CustomerItem
    {
        public int ID { get; set; }
        public string PublicIdentity { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public byte? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string PeoplesIdentity { get; set; }
        public string Password { get; set; }
        public string PassReal { get; set; }
        public string PasswordSalt { get; set; }
        public string Mobile { get; set; }
        public string Job { get; set; }
        public string Address { get; set; }
        public string LastIpAddress { get; set; }
        public string MacAddress { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }
        public string FaceBookId { get; set; }
        public string LinkFaceBook { get; set; }
        public string GoogleId { get; set; }
        public bool? IsActivated { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsLocked { get; set; }
        public string UrlPicture { get; set; }
        public string AlbumPictureJson { get; set; }
        public string DataJson { get; set; }
        public bool? IsOnline { get; set; }
        public bool? IsReceiveSale { get; set; }
        public string LikedProductIds { get; set; }
        public string BuyLaterProductIds { get; set; }
        public string ZaloId { get; set; }
        public string DeletedNotificationIds { get; set; }
        public string ReadNotificationIds { get; set; }
        public string CategoryIds { get; set; }
        public string CartCookies { get; set; }
        public string OAuthCodeZalo { get; set; }
        public string ZaloUid { get; set; }
        public string RoleModuleIds { get; set; }
    }
}