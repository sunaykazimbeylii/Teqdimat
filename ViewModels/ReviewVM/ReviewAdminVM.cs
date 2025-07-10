namespace TravelFinalProject.ViewModels
{
    public class ReviewAdminVM
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public string LangCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
