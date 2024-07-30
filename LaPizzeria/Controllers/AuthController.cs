using Microsoft.AspNetCore.Mvc;

namespace LaPizzeria.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        
    }
}
