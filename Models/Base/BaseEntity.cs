namespace TravelFinalProject.Models.Base
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
