﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using enBask.Core.Website.Models;
using enBask.Core.Website.Models.ManageViewModels;
using enBask.Core.Website.Services;
using enBask.Core.Website.Authentication;

namespace enBask.Core.Website.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly MyUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public ManageController(
        MyUserManager userManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            //             var model = new IndexViewModel
            //             {
            //                 HasPassword = await _userManager.HasPasswordAsync(user),
            //                 PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
            //                 TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
            //                 Logins = await _userManager.GetLoginsAsync(user),
            //                 BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            //             };

            var model = new IndexViewModel
            {
                HasPassword = true
            };
            return View(model);
        }


        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
//                 var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
//                 if (result.Succeeded)
//                 {
//                     await _signInManager.SignInAsync(user, isPersistent: false);
//                     _logger.LogInformation(3, "User changed their password successfully.");
//                     return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
//                 }
//                 AddErrors(result);
//                 return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
//                 var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
//                 if (result.Succeeded)
//                 {
//                     await _signInManager.SignInAsync(user, isPersistent: false);
//                     return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
//                 }
//                 AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.LookupByName(HttpContext.User.Identity.Name);
        }

        #endregion
    }
}
