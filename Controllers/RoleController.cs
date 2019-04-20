using FinanceRequest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceRequest.Controllers
{
    public class RoleController : Controller
    {
        ApplicationDbContext context;
        public RoleController()
        {
            context = new ApplicationDbContext();
        }


        // GET: Role
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {


                //if (!isAdminUser())
                //{
                //    return RedirectToAction("Index", "Home");
                //}
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Roles = context.Roles.ToList();
            return View(Roles);

        }
    }
}