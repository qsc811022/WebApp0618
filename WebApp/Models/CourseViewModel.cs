using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CourseViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, 99999)]
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }
}
