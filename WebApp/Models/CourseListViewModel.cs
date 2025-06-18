namespace WebApp.Models
{
    public class CourseListViewModel
    {
        public List<CourseCategory> Categories { get; set; }
        public List<Course> Courses { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
