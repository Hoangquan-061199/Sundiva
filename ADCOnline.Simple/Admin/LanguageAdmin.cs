namespace ADCOnline.Simple.Admin
{
    public class LanguageAdmin : BaseSimple
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public int OrderDisplay { get; set; }
        public string UrlPicture { get; set; }
    }
}
