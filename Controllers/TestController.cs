
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Repository; // Assuming your DbContext is in Repository namespace
using System.Net.Mail;
using System.Net;
using System.Linq;
using RMS.Models;
using Microsoft.Extensions.Configuration;


public class TestController : Controller
{
    public IActionResult Index()
    {
        return Content("Test controller is working.");
    }
}
