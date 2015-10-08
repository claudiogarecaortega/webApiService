
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientWebApi
{
    public class UserInformationViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public string IdDevice { get; set; }
        public string IdApplication { get; set; }
        public string UserKey { get; set; }
    }
    public class UserLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IdDevice { get; set; }
        public string Pin { get; set; }
    }
    public class PinViewModel
    {
        public string Email { get; set; }
        public string Pin { get; set; }
        public string IdDevice { get; set; }
        public string UserKey { get; set; }
    }
    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }





        static async Task RunAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("http://webapisecretmessage.azurewebsites.net//");




                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var login = new UserLoginViewModel() {Email = "asasas",IdDevice = "wwystg5",Pin = "1234",Password = "asdasdasdas"};
                // HTTP GET
                HttpResponseMessage response = await client.PostAsJsonAsync("api/UserInformation/Login",login);
                if (response.StatusCode==HttpStatusCode.Accepted)
                {
                    UserInformationViewModel product = await response.Content.ReadAsAsync<UserInformationViewModel>();
                    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Email, product.IdDevice);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                    Console.WriteLine("error de dispocitivo");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    Console.WriteLine("error de usuario o pass");


                var pin = new PinViewModel() { Email = "asasas", IdDevice = "wwystg5", Pin = "1234", UserKey = "/CvtahQdt2cVyJ2iot3Iv+vY4SbSwfBZC9cL83UySWo=" };
                // HTTP GET
                 response = await client.PostAsJsonAsync("api/UserInformation/Post", pin);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    UserInformationViewModel product = await response.Content.ReadAsAsync<UserInformationViewModel>();
                    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Email, product.IdDevice);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                    Console.WriteLine("error de dispocitivo");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    Console.WriteLine("error de usuario o pass");

                // HTTP POST
                var gizmo = new UserInformationViewModel() { Name = "Gizmo", Email = "asasas", IdDevice = "wwystg5", Pin = "1234", UserKey = "Widget",Password = "asdasdasdas"};
                response = await client.PostAsJsonAsync("api/UserInformation/PostPerson", gizmo);
                
                if (response.IsSuccessStatusCode)
                {
                    Uri gizmoUrl = response.Headers.Location;
                    var read = response.Content.ReadAsAsync<UserInformationViewModel>().Result;
                    
                    // HTTP PUT
                    gizmo.Email = "sdsadasd";   // Update price

                    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                    // HTTP DELETE
                    response = await client.DeleteAsync(gizmoUrl);
                }
            }
        }

    }
}
