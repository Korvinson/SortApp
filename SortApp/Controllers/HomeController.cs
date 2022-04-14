using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SortApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SortApp.Controllers
{
    public class HomeController : Controller
    {
        UsersContext db;
        

        public HomeController(UsersContext context)
        {
            this.db = context;
            if(db.Companies.Count() == 0)
            {
                Company oracle = new Company { Name = "Oracle" };
                Company google = new Company { Name = "Google" };
                Company microsoft = new Company { Name = "Microsoft" };
                Company apple = new Company { Name = "Apple" };

                User user1 = new User { Company = oracle, Name = "Олег Васильков", Age = 26 };
                User user2 = new User { Company = oracle, Name = "Александр Овсов", Age = 24 };
                User user3 = new User { Company = microsoft, Name = "Алексей Петров", Age = 25 };
                User user4 = new User { Company = microsoft, Name = "Иван Иванов", Age = 26 };
                User user5 = new User { Name = "Петр Андреев", Company = microsoft, Age = 23 };
                User user6 = new User { Name = "Василий Иванов", Company = google, Age = 23 };
                User user7 = new User { Name = "Олег Кузнецов", Company = google, Age = 25 };
                User user8 = new User { Name = "Андрей Петров", Company = apple, Age = 24 };

                db.Companies.AddRange(oracle, microsoft, google, apple);
                db.Users.AddRange(user1, user2, user3, user4, user5, user6, user7, user8);
                db.SaveChanges();
            }
        }

        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<User> users = db.Users.Include(x => x.Company);

            ViewData["NameSort"] = sortOrder==SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;

            users = sortOrder switch
            {
                SortState.NameDesc => users.OrderByDescending(x => x.Name),
                SortState.AgeAsc => users.OrderBy(x => x.Age),
                SortState.AgeDesc => users.OrderByDescending(x => x.Age),
                SortState.CompanyAsc => users.OrderBy(x => x.Company.Name),
                SortState.CompanyDesc => users.OrderByDescending(x => x.Company.Name),
                _ => users.OrderBy(x => x.Name),
            };
            return View(await users.AsNoTracking().ToListAsync());

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
