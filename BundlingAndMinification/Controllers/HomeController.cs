using BundlingAndMinification.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BundlingAndMinification.Services;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using BundlingAndMinification.Data;

namespace BundlingAndMinification.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;
        private readonly DbEmployeeContext _context;

        public HomeController(ILogger<HomeController> logger, IDistributedCache cache, DbEmployeeContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;

        }

        //[Compress]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Second()
        {
            return View();
        }
        
        [ResponseCache(Duration = 6, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult GetTime()
        {
            return View("Clock");
        }

        public async Task<IActionResult> GetEmployees()
        {
            var employees =  await _context.Employees.ToListAsync();
            return View("Employees", employees);
        }
        public async Task<IActionResult> GetEmployeesByRedis()
        {
            var cache_key = "GET_ALL_EMPLOYEES";
            List<Employee> employeesByRedis = new List<Employee>();
            var cacheData = await _cache.GetAsync(cache_key);
            if (cacheData != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(cacheData);
                employeesByRedis = JsonConvert.DeserializeObject<List<Employee>>(cachedDataString);
            }
            else
            {
                employeesByRedis = _context.Employees.ToList();
                var cachedDataString = JsonConvert.SerializeObject(employeesByRedis);
                var newDataToCache = Encoding.UTF8.GetBytes(cachedDataString);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                await _cache.SetAsync(cache_key, newDataToCache, options);
            }
            return View("Employees",employeesByRedis);
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
