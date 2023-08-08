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

        [Route("/Users/Statistics")]
        public IActionResult Statistics()
        {
            var usersWithMessages = _context.Users.Include(u => u.Messages);

            //Order users by number of messages
            ViewData["UserByMessages"] = usersWithMessages.OrderByDescending(user => user.Messages.Count()).ToList();

            //Most commonly used word overall
            ViewData["MostCommonWord"] = usersWithMessages
                .AsEnumerable()
                .SelectMany(message => message.Content.Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(word => word.ToLower())
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .First();

            //Most commonly used word by user
            ViewData["MostCommonWordByUser"] = usersWithMessages
                .AsEnumerable()
                .GroupBy(message => message.User)
                .Select(group => new
                {
                    User = group.Key,
                    MostCommonWord = group
                    .SelectMany(message => message.Content.Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .GroupBy(word => word.ToLower())
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .First()
                });
            return View(usersWithMessages);
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

        // GET: /Users/1
        [Route("/Users/{userId:int}")]
        public IActionResult Show(int userId)
        {
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Messages)
                .FirstOrDefault();

            return View(user);
        }
    }
}
