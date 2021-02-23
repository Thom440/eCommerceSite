using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceSite.Controllers
{
    public class UserController : Controller
    {
        private readonly ProductContext _context;

        public UserController(ProductContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel reg)
        {
            if (ModelState.IsValid)
            {
                // Map data to user account instance
                UserAccount acc = new UserAccount()
                {
                    DateOfBirth = reg.DateOfBirth,
                    Email = reg.Email,
                    Password = reg.Password,
                    Username = reg.Username
                };

                // Add to database
                _context.UserAccounts.Add(acc);
                await _context.SaveChangesAsync();

                // Redirect to homepage
                return RedirectToAction("Index", "Home");
            }
            return View(reg);
        }

        public IActionResult Login()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("UserID").HasValue)
            {
                return RedirectToAction("Index", "Home");
            } 

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserAccount account =
                await (from u in _context.UserAccounts
                       where (u.Username == model.UsernameOrEmail
                           || u.Email == model.UsernameOrEmail)
                           && u.Password == model.Password
                       select u).SingleOrDefaultAsync();
            if (account == null)
            {
                // Credentials did not match

                // Custom error message
                ModelState.AddModelError(string.Empty, "Credentials were not found");

                return View(model);
            }

            // Log user into website
            HttpContext.Session.SetInt32("UserID", account.UserID);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Removes all current session data
            HttpContext.Session.Clear();

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
    }
}
