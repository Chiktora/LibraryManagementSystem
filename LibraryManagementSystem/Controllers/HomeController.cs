using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManagementSystem.Controllers
{
    /// <summary>
    /// Controller responsible for handling the main navigation and general pages of the application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the HomeController.
        /// </summary>
        /// <param name="logger">The logger instance for recording diagnostic information.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page of the application.
        /// This is the default landing page for users.
        /// </summary>
        /// <returns>The home page view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// Contains information about data handling and user privacy.
        /// </summary>
        /// <returns>The privacy policy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Handles errors that occur during application execution.
        /// Displays a user-friendly error page with diagnostic information.
        /// </summary>
        /// <returns>An error view with details about the error that occurred.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
