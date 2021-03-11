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

        /// <summary>
        /// Displays a view to allow a user to register an
        /// account on the website
        /// </summary>
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel reg)
        {
            if (ModelState.IsValid)
            {
                // Check if username/email is in use
                bool isEmailTaken = await (from account in _context.UserAccounts
                                           where reg.Email == account.Email
                                           select account).AnyAsync();

                // if so, add custom error and send back to view
                if (isEmailTaken)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Email), "That email is already in use");
                }

                bool isUsernameTaken = await (from account in _context.UserAccounts
                                              where account.Username == reg.Username
                                              select account).AnyAsync();

                if (isUsernameTaken)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Username), "That username is already in use");
                }

                if (isEmailTaken || isUsernameTaken)
                {
                    return View(reg);
                }

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

                // Automatically log user in after registration
                LogUserIn(acc.UserID);

                // Redirect to homepage
                return RedirectToAction("Index", "Home");
            }
            return View(reg);
        }

        /// <summary>
        /// Displays a view to allow a user to login
        /// </summary>
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
            LogUserIn(account.UserID);

            return RedirectToAction("Index", "Home");
        }

        private void LogUserIn(int accountId)
        {
            HttpContext.Session.SetInt32("UserID", accountId);
        }

        public IActionResult Logout()
        {
            // Removes all current session data
            HttpContext.Session.Clear();

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
    }
}
