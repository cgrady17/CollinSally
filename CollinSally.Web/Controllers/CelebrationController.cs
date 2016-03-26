using CollinSally.Web.Models;
using CollinSally.Web.ViewModels;
using Grady.Framework.Mvc;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CollinSally.Web.Controllers
{
    public class CelebrationController : Controller
    {
        [HttpGet]
        public ActionResult Rsvp()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Rsvp(RsvpViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Website) || model.CaptchaPass == null || model.CaptchaPass != "COLLINSALLY_NOT_ROBOT")
            {
                return Json(new { status = 0, error = "Only humans can submit this form." });
            }

            using (CollinSallyWedding db = new CollinSallyWedding())
            using (DbContextTransaction dbTrans = db.Database.BeginTransaction())
            {
                RSVP rsvp = new RSVP
                {
                    Attending = model.Attending,
                    EmailAddress = model.EmailAddress
                };

                db.RSVPs.Add(rsvp);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();

                    return Json(new { status = 0, error = ex.Message });
                }

                RSVPAttendee primeAttendee = new RSVPAttendee
                {
                    EmailAddress = model.EmailAddress,
                    Name = model.Name,
                    RSVP_ID = rsvp.ID
                };

                db.RSVPAttendees.Add(primeAttendee);

                foreach (RSVPAttendee dbAttendee in model.Attendees.Where(attendee => attendee.EmailAddress != null).Select(attendee => new RSVPAttendee
                {
                    EmailAddress = attendee.EmailAddress,
                    Name = attendee.Name,
                    RSVP_ID = rsvp.ID
                }))
                {
                    db.RSVPAttendees.Add(dbAttendee);
                }

                try
                {
                    await db.SaveChangesAsync();
                    dbTrans.Commit();
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();

                    return Json(new { status = 0, error = ex.Message });
                }

                await SendRsvpEmailAsync(model);
            }

            return Json(new { status = 1 });
        }

        [HttpGet]
        public ActionResult Location()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Photos()
        {
            return View();
        }

        #region Manage RSVPs

        public ActionResult Manage()
        {
            bool? isAuthenticated = Session["IsAuthenticated"] as bool?;

            bool? badPass = TempData["BadPass"] as bool?;

            if (badPass.HasValue && badPass.Value) ViewBag.BadPass = true;

            return View(isAuthenticated.HasValue && isAuthenticated.Value ? "Manage" : "SecurityCheck");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Manage(string password)
        {
            if (!string.IsNullOrWhiteSpace(password) && password == "Maggie2016")
            {
                Session["IsAuthenticated"] = true;
            }
            else
            {
                TempData["BadPass"] = true;
            }

            return Manage();
        }

        public async Task<JsonResult> Read(int? id)
        {
            using (CollinSallyWedding db = new CollinSallyWedding())
            {
                switch (id)
                {
                    case 1:
                        return Json(await db.RSVPs.CountAsync(), JsonRequestBehavior.AllowGet);

                    case 2:
                        return Json(await db.RSVPs.CountAsync(x => x.Attending), JsonRequestBehavior.AllowGet);

                    case 3:
                        return Json(await db.RSVPs.CountAsync(x => !x.Attending), JsonRequestBehavior.AllowGet);
                }

                List<RsvpExistingViewModel> existingRsvps = await (from ra in db.RSVPAttendees
                                                                   join r in db.RSVPs on ra.RSVP_ID equals r.ID
                                                                   select new RsvpExistingViewModel
                                                                   {
                                                                       ID = r.ID,
                                                                       Name = ra.Name,
                                                                       EmailAddress = ra.EmailAddress,
                                                                       Attending = r.Attending
                                                                   }).ToListAsync();

                return Json(new
                {
                    data = existingRsvps
                        .Distinct()
                        .Select(x => new
                        {
                            x.ID,
                            x.Name,
                            EmailAddress = "<a href='mailto:" + x.EmailAddress + "'>" + x.EmailAddress + "</a>",
                            Attending = x.Attending ? "Yes" : "No"
                        })
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> TestEmail()
        {
            await SendRsvpEmailAsync(new RsvpViewModel
            {
                Attendees = new List<RsvpAttendeeViewModel>
                {
                    new RsvpAttendeeViewModel
                    {
                        EmailAddress = "cgrady17@outlook.com",
                        Name = "Connor Grady"
                    }
                },
                Attending = true,
                EmailAddress = "cgrady17@outlook.com",
                Name = "Connor Grady",
                OtherAttendees = false
            });

            return HttpNotFound();
        }

        private async Task SendRsvpEmailAsync(RsvpViewModel model)
        {
            SendGridMessage message = new SendGridMessage
            {
                From = new MailAddress("sally-collin@collinsally.com", "Sally & Collin Grady"),
                Subject = "RSVP Confirmation | Sally & Collin's Celebration",
                Html = this.RenderPartialViewToString("RSVPEmail", model)
            };

            message.AddTo($"{model.Name} <{model.EmailAddress}>");

            SendGrid.Web transportWeb = new SendGrid.Web(ConfigurationManager.AppSettings["sendgrid:APIKey"]);

            await transportWeb.DeliverAsync(message);
        }
    }
}