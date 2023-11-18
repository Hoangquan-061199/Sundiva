using System.Collections.Generic;
using ADCOnline.Simple.Item;

namespace Website.ViewModels
{
    public class CommentViewModel
    {
        public bool errors { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int parentId { get; set; }
        public int contentId { get; set; }
        public int productId { get; set; }
        public IEnumerable<CommentItem> CommentItems { get; set; }
        public string GridHtml { get; set; }
    }
}
