using Microsoft.AspNetCore.Mvc;

using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly CourseRepository _repo;

        public TeacherController(CourseRepository repo)
        {
            _repo = repo;
        }

        // GET: /Teacher
        public async Task<IActionResult> Index()
        {
            var teachers = await _repo.GetAllTeachersAsync();
            return View(teachers);
        }
    }
}
