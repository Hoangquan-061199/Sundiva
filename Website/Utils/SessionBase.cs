using Microsoft.AspNetCore.Http;
namespace Website.Utils
{
    public class SessionBase
    {
        readonly HttpContext context;
        public SessionBase(HttpContext httpContext) => context = httpContext;
        public void SetAdminUserId(string id) => context.Session.SetString("WebAdminUserID", id);
        public string GetAdminUserId() => context.Session.GetString("WebAdminUserID");
        public void SetAdminUserName(string userName) => context.Session.SetString("WebAdminUserName", userName);
        public string GetAdminUserName()
        => context.Session.GetString("WebAdminUserName");
        public void SetAdminRole(string RoleCode)
        => context.Session.SetString("WebAdminRole", RoleCode);
        public string GetAdminRole()
        => context.Session.GetString("WebAdminRole");
        public  void SetWebVerify(string value)
        => context.Session.SetString("WebVerify", value);
        public string GetWebVerify()
        => context.Session.GetString("WebVerify");
        #region  Session
        public void SetSessionUserId(int id)
        => context.Session.SetString("SetSessionUserId", id.ToString());
        public void SetSessionAccount(string id)
        => context.Session.SetString("SetSessionAccount", id);
        public string GetSessionUserId()
        => context.Session.GetString("SetSessionUserId");
        public string GetSessionAccount()
        => context.Session.GetString("SetSessionAccount");
        #endregion
    }
}