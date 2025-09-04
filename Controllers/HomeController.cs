using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RMS.Web.Models;
using RMS.Data.Services;
using RMS.Data.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace RMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    //private readonly IReviewService svc ;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        //svc = reviewService;

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {

        var about = new AboutViewModel
        {
            Title = "About",
            Message = "Our mission is to develop great recipes for healthy eating",
            Formed = new DateTime(2000, 10, 1)

        };
        return View(about);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


// public IActionResult Chatbot()
// {
//     return View();
// }


}
