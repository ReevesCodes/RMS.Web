using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RMS.Web.Controllers;

public enum AlertType { success, danger, warning, info }

// Implements General functionality which is then accessible to any 
// Controller inheriting from BaseController
public class BaseController : Controller
{
    // set alert message
    public void Alert(string message, AlertType type = AlertType.info)
    {
        TempData["Alert.Message"] = message;
        TempData["Alert.Type"] = type.ToString();
    }


    protected string GetSignedInUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            return userId;
        }
        // throw new Exception("User ID claim is missing or invalid.");
        return null;
    }


    // check if user us currently authenticated
    public bool IsAuthenticated()
    {
        return User.Identity.IsAuthenticated;
    }



    public IActionResult CheckAuthentication()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Content("User is NOT authenticated");
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Content("User is authenticated, but User ID claim is missing or invalid.");
        }

        var userId = userIdClaim.Value;
        return Content($"User is authenticated. User ID: {userId}");
    }

public IActionResult ShowClaims()
{
    var claims = User.Claims.Select(c => $"{c.Type} = {c.Value}");
    return Content(string.Join("\n", claims));
}


}




// using System.Linq;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Mvc;

// namespace RMS.Web.Controllers;

// public enum AlertType { success, danger, warning, info }

// public class BaseController : Controller
// {
//     public void Alert(string message, AlertType type = AlertType.info)
//     {
//         TempData["Alert.Message"] = message;
//         TempData["Alert.Type"] = type.ToString();
//     }

//     protected string GetSignedInUserId()
//     {
//         return User?.FindFirstValue(ClaimTypes.NameIdentifier); // null-safe
//     }

//     public bool IsAuthenticated()
//     {
//         return User?.Identity?.IsAuthenticated ?? false;
//     }

//     public IActionResult CheckAuthentication()
//     {
//         if (!IsAuthenticated())
//         {
//             return Content("User is NOT authenticated");
//         }

//         var userId = GetSignedInUserId();
//         return Content(string.IsNullOrEmpty(userId) ? 
//             "User is authenticated, but User ID claim is missing or invalid." : 
//             $"User is authenticated. User ID: {userId}");
//     }

//     public IActionResult ShowClaims()
//     {
//         var claims = User?.Claims?.Select(c => $"{c.Type} = {c.Value}") ?? Enumerable.Empty<string>();
//         return Content(string.Join("\n", claims));
//     }
// }
