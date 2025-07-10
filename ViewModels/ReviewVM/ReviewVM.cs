using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.ReviewVM
{
    public class ReviewVM
    {
        public int TourId { get; set; }
        [ValidateNever]
        public string TourTitle { get; set; }

        public string Comment { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string LengCode { get; set; }
    }
}
