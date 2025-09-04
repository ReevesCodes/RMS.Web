using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Repository; // Assuming your DbContext is in Repository namespace
using System.Net.Mail;
using System.Net;
using System.Linq;
using RMS.Models;
using Microsoft.Extensions.Configuration;


public class SupportController : Controller
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public SupportController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SupportMessage());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(SupportMessage model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        // Save message to database
        model.SubmittedAt = DateTime.UtcNow;
        _context.SupportMessages.Add(model);
        _context.SaveChanges();

        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

            var fromAddress = new MailAddress(emailSettings.SupportEmail, "Mandy's Support");
            var toAddress = new MailAddress(emailSettings.SendTo, "Admin");

            string subject = "New Customer Support Message";
            string body = $"From: {model.Name}\nEmail: {model.Email}\n\nMessage:\n{model.Message}";

            var smtp = new SmtpClient
            {
                Host = emailSettings.SmtpHost,
                Port = emailSettings.SmtpPort,
                EnableSsl = emailSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.SupportEmail, emailSettings.SupportPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
            ViewBag.Status = "✅ Your message has been received and emailed to our support team!";
        }
        catch (Exception ex)
        {
            ViewBag.Status = "⚠️ Your message was saved, but we couldn’t send an email. Please try again later.";
            Console.WriteLine("Email Error: " + ex.Message); // Or use a logger
        }

        return View("Index", new SupportMessage());
    }

    [HttpGet]
    public IActionResult History()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var messages = _context.SupportMessages
            .Where(m => m.Email == email)
            .OrderByDescending(m => m.SubmittedAt)
            .ToList();

        return View(messages);
    }
    

    ///// this is to be remmoved after testing the email sent succesfully
    [HttpGet]
public IActionResult TestEmail()
{
    try
    {
        var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
        var fromAddress = new MailAddress(emailSettings.SupportEmail, "Test");
        var toAddress = new MailAddress(emailSettings.SendTo);

        var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort)
        {
            EnableSsl = emailSettings.EnableSsl,
            Credentials = new NetworkCredential(emailSettings.SupportEmail, emailSettings.SupportPassword)
        };

        var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = "Test Email",
            Body = "This is a test email."
        };

        smtp.Send(message);
        return Content("Test email sent successfully.");
    }
    catch (Exception ex)
    {
        return Content("Test email error: " + ex.ToString());
    }
}

}






// using Microsoft.AspNetCore.Mvc;
// using RMS.Data.Entities;
// using RMS.Data.Repository;
// using RMS.Models;
// using System.Net.Mail;
// using System.Net;
// using Microsoft.EntityFrameworkCore;

// public class SupportController : Controller
// {
//     private readonly DataContext _context;
//     private readonly IConfiguration _configuration;

//     public SupportController(DataContext context, IConfiguration configuration)
//     {
//         _context = context;
//         _configuration = configuration;
//     }

//     // GET: /Support
//     [HttpGet]
//     public IActionResult Index()
//     {
//         return View(new SupportMessage());
//     }

//     // POST: /Support/Submit
//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public IActionResult Submit(SupportMessage model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return View("Index", model);
//         }

//         // Save the support message
//         model.SubmittedAt = DateTime.UtcNow;
//         _context.SupportMessages.Add(model);
//         _context.SaveChanges();

//         // Send email
//         try
//         {
//             var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
//             var fromAddress = new MailAddress(emailSettings.SupportEmail, "Mandy's Support");
//             var toAddress = new MailAddress(emailSettings.SendTo, "Admin");

//             string subject = "New Customer Support Message";
//             string body = $"From: {model.Name}\nEmail: {model.Email}\n\nMessage:\n{model.Message}";

//             var smtp = new SmtpClient
//             {
//                 Host = emailSettings.SmtpHost,
//                 Port = emailSettings.SmtpPort,
//                 EnableSsl = emailSettings.EnableSsl,
//                 DeliveryMethod = SmtpDeliveryMethod.Network,
//                 UseDefaultCredentials = false,
//                 Credentials = new NetworkCredential(emailSettings.SupportEmail, emailSettings.SupportPassword)
//             };

//             using var message = new MailMessage(fromAddress, toAddress)
//             {
//                 Subject = subject,
//                 Body = body
//             };

//             smtp.Send(message);

//             // ✅ Use TempData so alert survives redirect
//             TempData["Status"] = "✅ Your message has been received and emailed to our support team!";
//         }
//         catch
//         {
//             TempData["Status"] = "⚠️ Your message was saved, but we couldn’t send an email. Please try again later.";
//         }

//         // Redirect to Index so TempData alert shows
//         return RedirectToAction("Index");
//     }

//     // GET: /Support/History
//     [HttpGet]
//     public IActionResult History(string email = null)
//     {
//         string userEmail = User.Identity?.Name ?? email;

//         if (string.IsNullOrEmpty(userEmail))
//         {
//             TempData["Status"] = "Please submit a message first or provide your email to see your history.";
//             return RedirectToAction("Index");
//         }

//         var messages = _context.SupportMessages
//             .Where(m => m.Email == userEmail || (m.User != null && m.User.Email == userEmail))
//             .OrderByDescending(m => m.SubmittedAt)
//             .ToList();

//         return View(messages); // History.cshtml
//     }
// }
