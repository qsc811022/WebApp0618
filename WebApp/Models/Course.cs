namespace WebApp.Models
{
    public class Comment
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int TeacherId { get; set; }            // ← 新增
        public int CategoryId { get; set; }           // ← 新增
        public string TeacherName { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Syllabus { get; set; }
        public string LearningGoals { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
