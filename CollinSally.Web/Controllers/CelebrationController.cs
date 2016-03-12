using CollinSally.Web.Models;
using CollinSally.Web.ViewModels;
using Grady.Framework.Mvc;
using SendGrid;
using System;
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

        private async Task SendRsvpEmailAsync(RsvpViewModel model)
        {
            SendGridMessage message = new SendGridMessage
            {
                From = new MailAddress("sally-collin@collinsally.com", "Sally & Collin"),
                Subject = "RSVP Confirmation | Sally & Collin's Celebration",
                Html = this.RenderPartialViewToString("RSVPEmail", model)
            };

            message.AddTo($"{model.Name} <{model.EmailAddress}>");

            SendGrid.Web transportWeb = new SendGrid.Web(ConfigurationManager.AppSettings["sendgrid:APIKey"]);

            await transportWeb.DeliverAsync(message);
        }
    }
}