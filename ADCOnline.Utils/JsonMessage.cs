using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System;

namespace ADCOnline.Utils
{
    [Serializable]
    public class JsonMessage
    {
        #region các biến
        public bool Errors { get; set; }
        public int Errorcode { get; set; }
        public string Errormessage { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public string Css { get; set; }
        public string Email { get; set; }
        public string Logs { get; set; }
        public string Name { get; set; }
        public Comment Data { get; set; }
        public ProductDetail Product { get; set; }
        public object Obj { get; set; }
        public int Number { get; set; }
        public string Action { get; set; }
        public string ProductId { get; set; }
        #endregion
    }
}
