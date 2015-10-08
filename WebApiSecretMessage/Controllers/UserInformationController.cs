using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using WebApiSecretMessage.Models;

namespace WebApiSecretMessage.Controllers
{
    public class UserInformationController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static string GenerateUserKey(string pin, string email, string idDevice)
        {
            return Sha256Encrypt(pin+email+idDevice);
        }
        public static string Sha256Encrypt(string phrase)
        {
            var encoder = new UTF8Encoding();
            var sha256Hasher = new SHA256Managed();
            var hashedDataBytes = sha256Hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
        [HttpPost]
        [ActionName("Login")]
        public HttpResponseMessage Login([FromBody]UserLoginViewModel viewModel)
        {
             if (ModelState.IsValid)
            {

                var user =db.UserInformations.AsEnumerable().FirstOrDefault(x => x.Email==viewModel.Email && x.Password==viewModel.Password);
                if(user!=null && user.IdDevice==viewModel.IdDevice)
                    return Request.CreateResponse(HttpStatusCode.Accepted,user);
                else if (user.IdDevice != viewModel.IdDevice)
                    return Request.CreateResponse(HttpStatusCode.Conflict,user);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound,user);


               
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [HttpPost]
        [ActionName("Pin")]
        public HttpResponseMessage Pin(PinViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                var user = db.UserInformations.AsEnumerable().FirstOrDefault(x => x.Email == viewModel.Email);
                if (user != null && user.Email == viewModel.Email && user.Pin == viewModel.Pin && user.IdDevice == viewModel.IdDevice && user.UserKey==viewModel.UserKey)
                    return Request.CreateResponse(HttpStatusCode.Accepted,user);
                else if (user.IdDevice != viewModel.IdDevice)
                    return Request.CreateResponse(HttpStatusCode.Conflict,user);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound,user);



            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpPost]
        [ActionName("Recovery")]
        public HttpResponseMessage Recovery(RecoveryViewModel person)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Person
        private string GenerateGuid()
        {
            while (true)
            {
                var id = Guid.NewGuid().ToString();
                if (!db.UserInformations.Any(x => x.IdApplication == id))
                    return id;
            }
        }

        public HttpResponseMessage PostPerson(UserInformationViewModel person)
        {
            
            if (ModelState.IsValid)
            {
                var validMail = db.UserInformations.AsQueryable().Any(x => x.Email == person.Email);
                if(validMail)
                    return Request.CreateResponse(HttpStatusCode.Conflict);

                var UserKey = GenerateUserKey(person.Pin, person.Email, person.IdDevice);
                var model = new UserInformation
                {
                    Name = person.Name,
                    Email = person.Email,
                    Password = person.Password,
                    IdDevice = person.IdDevice,
                    Pin = person.Pin,
                    UserKey = UserKey,
                    IdApplication = GenerateGuid()//GenerateUserKey(person.Pin, person.Email, UserKey)
                   // Id = new Guid().ToString()

                };

                db.UserInformations.Add(model);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
               return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}