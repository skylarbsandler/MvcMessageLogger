using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class UsersController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public UsersController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users;
            return View(users);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Route("/users/")]
        public IActionResult Create(User user)
        {
            _context.Add(user);
            _context.SaveChanges();

            //var newUserId = user.Id;

            return RedirectToAction("index");
        }
    }
}
