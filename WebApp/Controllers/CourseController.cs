using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{

    public class CourseController : Controller
    {
        private readonly CourseRepository _courseRepo;

        public CourseController(CourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }
    


        public async Task<IActionResult> Index(string? keyword, string? sort)
        {
            var courses = await _courseRepo.GetAllCoursesAsync();

            // 過濾
            if (!string.IsNullOrEmpty(keyword))
            {
                courses = courses
                    .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword))
                    .ToList();
            }

            // 排序
            courses = sort switch
            {
                "priceAsc" => courses.OrderBy(c => c.Price).ToList(),
                "priceDesc" => courses.OrderByDescending(c => c.Price).ToList(),
                _ => courses
            };

            ViewData["Keyword"] = keyword;
            ViewData["Sort"] = sort;
            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        public async Task<IActionResult> Category(int categoryId)
        {
            var courses = await _courseRepo.GetCoursesByCategoryAsync(categoryId);
            ViewData["CategoryId"] = categoryId;
            return View("Category", courses);
        }
        public async Task<IActionResult> CategoryPage(string name, string? keyword, string? sort)
        {
            var (description, courses) = await _courseRepo.GetCategoryDetailAsync(name);

            // 過濾關鍵字
            if (!string.IsNullOrEmpty(keyword))
            {
                courses = courses
                    .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword))
                    .ToList();
            }

            // 排序邏輯
            courses = sort switch
            {
                "priceAsc" => courses.OrderBy(c => c.Price).ToList(),
                "priceDesc" => courses.OrderByDescending(c => c.Price).ToList(),
                _ => courses
            };

            ViewData["CategoryName"] = name;
            ViewData["CategoryDescription"] = description;
            return View("CategoryPage", courses);
        }
        // --- 這裡新增 Admin 專用 Action ---
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var courses = await _courseRepo.GetAllCoursesAsync();
            return View(courses); // 對應 Views/Course/Admin.cshtml
        }

        // GET: Course/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Teachers = await _courseRepo.GetAllTeachersAsync();
            ViewBag.Categories = await _courseRepo.GetAllCategoriesAsync();
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = await _courseRepo.GetAllTeachersAsync();
                ViewBag.Categories = await _courseRepo.GetAllCategoriesAsync();
                return View(model);
            }

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                TeacherId = model.TeacherId,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.Now
            };

            await _courseRepo.CreateCourseAsync(course);
            return RedirectToAction("Admin");
        }

        // GET: Course/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            // 載入老師與分類選單
            ViewBag.Teachers = await _courseRepo.GetAllTeachersAsync();
            ViewBag.Categories = await _courseRepo.GetAllCategoriesAsync();

            // 轉成 ViewModel（如果有用 CourseViewModel，否則直接回傳 course 也行）
            var model = new CourseViewModel
            {
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                TeacherId = course.TeacherId,
                CategoryId = course.CategoryId
            };

            return View(model);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = await _courseRepo.GetAllTeachersAsync();
                ViewBag.Categories = await _courseRepo.GetAllCategoriesAsync();
                return View(model);
            }

            // 更新資料
            await _courseRepo.UpdateCourseAsync(id, model);
            return RedirectToAction("Admin");
        }


        // GET: Course/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseRepo.DeleteCourseAsync(id);
            return RedirectToAction("Admin");
        }
        // CourseController.cs

        public async Task<IActionResult> Learning(int? categoryId)
        {
            var categories = await _courseRepo.GetAllCategoriesAsync();
            var courses = categoryId.HasValue
                ? await _courseRepo.GetCoursesByCategoryAsync(categoryId.Value)
                : await _courseRepo.GetAllCoursesAsync();

            var vm = new CourseListViewModel
            {
                Categories = categories.ToList(),
                Courses = courses.ToList(),
                SelectedCategoryId = categoryId
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentCreateViewModel vm)
        {
            // 這裡要判斷用戶有無登入，取 UserId，這裡假設你有 userId 變數
            int userId = 1; // 測試用，請改成從登入Session取得
            await _courseRepo.InsertCommentAsync(vm, userId);
            return RedirectToAction("Details", new { id = vm.CourseId });
        }


    }
}
