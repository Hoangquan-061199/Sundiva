using System; 
 namespace ADCOnline.Simple.Base {
public class AspnetUsers { public Guid ApplicationId { get; set; } 
 public Guid UserId { get; set; } 
 public string UserName { get; set; } 
 public string LoweredUserName { get; set; } 
 public string MobileAlias { get; set; } 
 public bool IsAnonymous { get; set; } 
 public DateTime LastActivityDate { get; set; } 
 public string TwoFactorSecret { get; set; } 
 public int? DepartmentID { get; set; } 
 public Guid? ParrentID { get; set; } 
 }}
