namespace DEWAXP.Foundation.Helpers.Models
{
    public class SanitizerOptions
    {
        public SanitizerOptions()
        {
            IsUrlEncode = true;
        }

        public bool AllowHtml { get; set; }
        public bool AllowScript { get; set; }
        public bool AllowStyle { get; set; }
        public bool AllowNewlineTab { get; set; }
        public bool AllowSpace { get; set; }
        public bool AllowAbnormalQuote { get; set; }
        public bool IsUrlEncode { get; set; }
    }
}