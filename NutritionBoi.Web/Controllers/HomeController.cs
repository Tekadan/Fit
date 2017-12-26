using NutritionBoi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NutritionBoi.Web.Controllers
{
    public class HomeController : Controller
    {

        // Below are the static names of the cookies
        private const string _macroCookie = "MacronutrientsModel";
        private const string _clientInfoCookie = "ClientInformationModel";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Calculate()
        {
            ClientInformationModel model = new ClientInformationModel();

            if (Request.Cookies[_clientInfoCookie] != null)
            {
                model = ClientInformationModel.FromCookie(Request.Cookies[_clientInfoCookie]);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Calculate(ClientInformationModel model)
        {
            // Validate Data

            // Read the old cookie so we can update if possible
            ClientInformationModel oldClientInfo = null;
            if (Request.Cookies[_clientInfoCookie] != null)
            {
                oldClientInfo = ClientInformationModel.FromCookie(Request.Cookies[_clientInfoCookie]);
            }

            MacronutrientsModel currentMacros = null;
            if (Request.Cookies[_macroCookie] != null)
            {
                currentMacros = MacronutrientsModel.FromCookie(Request.Cookies[_macroCookie]);
            }

            HttpCookie clientInfoCookie = ClientInformationModel.ToCookie(model);
            Response.AppendCookie(clientInfoCookie);

            MacronutrientsModel macros = null;

            if (oldClientInfo != null && currentMacros != null)
            {
                macros = MacronutrientsModel.Parse(Logic.MTL.UpdateMacros(currentMacros.ToMtlModel(), oldClientInfo.ToMtlModel(), model.ToMtlModel()));
            }
            else
            {
                macros = MacronutrientsModel.Parse(Logic.MTL.Calculate(model.ToMtlModel()));
            }

            HttpCookie currentMacrosCookie = MacronutrientsModel.ToCookie(macros);

            Response.AppendCookie(currentMacrosCookie);

            return RedirectToAction("CurrentMacros");
        }

        public ActionResult CurrentMacros()
        {
            MacronutrientsModel model = null;

            if (Request.Cookies[_macroCookie] != null)
            {
                model = MacronutrientsModel.FromCookie(Request.Cookies[_macroCookie]);

                return View(model);
            }

            return View(model);
        }

        public ActionResult DumpCookies()
        {
            Session.Abandon();
            List<HttpCookie> cookiesToDelete = new List<HttpCookie>();

            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                var cookie = Request.Cookies[i];
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookiesToDelete.Add(cookie);
            }

            foreach (var cookie in cookiesToDelete)
            {
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Calculate");
        }
    }
}