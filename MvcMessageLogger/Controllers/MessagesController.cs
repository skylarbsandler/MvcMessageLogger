using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public MessagesController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/users/{userId:int}")]
        public IActionResult Index(int userId, Message message)
        {
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Messages)
                .First();
            user.Messages.Add(message);
           
            _context.SaveChanges();

            //var newUserId = user.Id;

            return Redirect($"/users/{userId}");

            //return RedirectToAction("show", new { id= user.Id});
        }
    }
}
