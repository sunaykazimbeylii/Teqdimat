namespace TravelFinalProject.ViewModels
{
    public class PaginatedVM<T>
    {
        public PaginatedVM<GetTourVM> paginatedVM { get; set; }
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> Items { get; set; }
        public string LangCode { get; set; }
    }
}
