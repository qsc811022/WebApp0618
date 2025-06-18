using Dapper;

using System.Data;

using WebApp.Models;

namespace WebApp.Repositories
{
    public class CourseRepository
    {

        private readonly IDbConnection _db;

        public CourseRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            var sql = @"
            SELECT c.Id, c.Title, c.Description, c.Price, c.CreatedAt,
                   t.Name AS TeacherName,
                   cat.Name AS CategoryName
            FROM Courses c
            JOIN Teachers t ON c.TeacherId = t.Id
            JOIN CourseCategories cat ON c.CategoryId = cat.Id
            ORDER BY c.CreatedAt DESC;
        ";
            return await _db.QueryAsync<Course>(sql);
        }
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            var sql = @"
        SELECT c.Id, c.Title, c.Description, c.Price, c.CreatedAt,
               t.Name AS TeacherName,
               cat.Name AS CategoryName
        FROM Courses c
        JOIN Teachers t ON c.TeacherId = t.Id
        JOIN CourseCategories cat ON c.CategoryId = cat.Id
        WHERE c.Id = @Id;

        SELECT u.Name AS UserName, cm.Content, cm.Rating, cm.CreatedAt
        FROM Comments cm
        JOIN Users u ON cm.UserId = u.Id
        WHERE cm.CourseId = @Id
        ORDER BY cm.CreatedAt DESC;
    ";

            using var multi = await _db.QueryMultipleAsync(sql, new { Id = id });
            var course = await multi.ReadSingleOrDefaultAsync<Course>();
            if (course != null)
            {
                course.Comments = (await multi.ReadAsync<Comment>()).ToList();
            }
            return course;
        }
        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            var sql = @"
        SELECT c.Id, c.Title, c.Description, c.Price, c.CreatedAt,
               t.Name AS TeacherName,
               cat.Name AS CategoryName
        FROM Courses c
        JOIN Teachers t ON c.TeacherId = t.Id
        JOIN CourseCategories cat ON c.CategoryId = cat.Id
        WHERE cat.Id = @CategoryId
        ORDER BY c.CreatedAt DESC;
    ";

            return await _db.QueryAsync<Course>(sql, new { CategoryId = categoryId });
        }

        public async Task<(string CategoryDesc, List<Course> Courses)> GetCategoryDetailAsync(string categoryName)
        {
            var sqlCategory = "SELECT Description FROM CourseCategories WHERE Name = @Name;";
            var sqlCourses = @"
        SELECT c.Id, c.Title, c.Description, c.Price, c.CreatedAt,
               t.Name AS TeacherName,
               cat.Name AS CategoryName
        FROM Courses c
        JOIN Teachers t ON c.TeacherId = t.Id
        JOIN CourseCategories cat ON c.CategoryId = cat.Id
        WHERE cat.Name = @Name
        ORDER BY c.CreatedAt DESC;
    ";

            var desc = await _db.QuerySingleOrDefaultAsync<string>(sqlCategory, new { Name = categoryName });
            var courses = (await _db.QueryAsync<Course>(sqlCourses, new { Name = categoryName })).ToList();

            return (desc, courses);
        }
        public async Task CreateCourseAsync(CourseViewModel model)
        {
            var sql = @"INSERT INTO Courses (Title, TeacherId, CategoryId, Price, Description, CreatedAt)
                VALUES (@Title, @TeacherId, @CategoryId, @Price, @Description, GETDATE())";
            await _db.ExecuteAsync(sql, model);
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            //var sql = "SELECT Id, Name FROM Teachers ORDER BY Name";
            var sql = "SELECT Id, Name, Subject, Description, PhotoUrl FROM Teachers ORDER BY Name";
            return await _db.QueryAsync<Teacher>(sql);
        }

        public async Task<IEnumerable<CourseCategory>> GetAllCategoriesAsync()
        {
            var sql = "SELECT Id, Name FROM CourseCategories ORDER BY Name";
            return await _db.QueryAsync<CourseCategory>(sql);
        }
        public async Task CreateCourseAsync(Course course)
        {
            var sql = @"INSERT INTO Courses (Title, Description, Price, TeacherId, CategoryId, CreatedAt)
                VALUES (@Title, @Description, @Price, @TeacherId, @CategoryId, @CreatedAt)";
            await _db.ExecuteAsync(sql, course);
        }

        public async Task UpdateCourseAsync(int id, CourseViewModel model)
        {
            var sql = @"UPDATE Courses
                SET Title = @Title,
                    Description = @Description,
                    Price = @Price,
                    TeacherId = @TeacherId,
                    CategoryId = @CategoryId
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, new
            {
                Id = id,
                model.Title,
                model.Description,
                model.Price,
                model.TeacherId,
                model.CategoryId
            });
        }

        public async Task DeleteCourseAsync(int id)
        {
            var sql = "DELETE FROM Courses WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        public async Task InsertCommentAsync(CommentCreateViewModel vm, int userId)
        {
            var sql = @"INSERT INTO Comments (CourseId, UserId, Content, Rating, CreatedAt)
                VALUES (@CourseId, @UserId, @Content, @Rating, GETDATE())";
            await _db.ExecuteAsync(sql, new { vm.CourseId, UserId = userId, vm.Content, vm.Rating });
        }


    }

}
