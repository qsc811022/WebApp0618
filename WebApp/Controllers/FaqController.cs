using Microsoft.AspNetCore.Mvc;

using WebApp.IRepository;

namespace WebApp.Controllers
{
    public class FaqController : Controller
    {
        private readonly IFaqRepository _faqRepo;
        public FaqController(IFaqRepository faqRepo)
        {
            _faqRepo = faqRepo;
        }

        // 顯示 FAQ 列表
        public async Task<IActionResult> Index()
        {
            var faqs = await _faqRepo.GetAllAsync();
            return View(faqs);
        }
    }
}
