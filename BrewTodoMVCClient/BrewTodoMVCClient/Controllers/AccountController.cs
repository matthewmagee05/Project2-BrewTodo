﻿using BrewTodoMVCClient.Logic;
using BrewTodoMVCClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BrewTodoMVCClient.Controllers
{
    public class AccountController : ServiceController
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            ViewBag.LogIn = CurrentUser.UserLoggedIn();
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<ActionResult> Login(Account account)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            HttpRequestMessage apiRequest = CreateRequestToService(HttpMethod.Post, "api/Account/Login");
            apiRequest.Content = new ObjectContent<Account>(account, new JsonMediaTypeFormatter());

            HttpResponseMessage apiResponse;
            try
            {
                apiResponse = await HttpClient.SendAsync(apiRequest);
       //         CurrentUser.currentUserId = apiResponse.Content.ToString();
            }
            catch
            {
                return View("Error");
            }

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("Error");
            }

            PassCookiesToClient(apiResponse);

            UserLogic logic = new UserLogic();
            ICollection<UserViewModel> users = logic.GetUsers();
            CurrentUser.currentUserId = users.Where(x => x.Username.ToUpper().Equals(account.Username.ToUpper())).FirstOrDefault().UserID;

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        public async Task<ActionResult> Logout()
        {
            ViewBag.LogIn = CurrentUser.UserLoggedIn();
            AccountLogic logic = new AccountLogic();

            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            HttpRequestMessage apiRequest = CreateRequestToService(HttpMethod.Get, "api/Account/Logout");

            HttpResponseMessage apiResponse;
            try
            {
                apiResponse = await HttpClient.SendAsync(apiRequest);
            }
            catch
            {
                return View("Error");
            }

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("Error");
            }

            logic.Logout();
            CurrentUser.currentUserId = null;
            PassCookiesToClient(apiResponse);

            return Redirect("http://angular.brewtodo.com");
        }
        private bool PassCookiesToClient(HttpResponseMessage apiResponse)
        {
            if (apiResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                foreach (string value in values)
                {
                    Response.Headers.Add("Set-Cookie", value);
                }
                return true;
            }
            return false;
        }
    }
}
