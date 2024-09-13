namespace DEWAXP.Foundation.Content.Models.Base
{
    public class PaginationModel
    {
        public PaginationModel(string action, string controller, int currentPage, int totalPages)
        {
            Controller = controller;
            Action = action;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }

        public virtual int CurrentPage { get; private set; }

        public virtual int TotalPages { get; private set; }

        public virtual string Action { get; private set; }

        public virtual string Controller { get; private set; }

        public virtual string QueryString { get; set; }

        public virtual string filter { get; set; }

        public virtual bool CanLoadMore
        {
            get { return CurrentPage < TotalPages; }
        }
    }
}