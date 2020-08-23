using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    class Login
    {
        public string AccessToken { get; set; }
    }

    public partial class Tevian
    {
        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="billingType"></param>
        /// <returns>User</returns>
        public static async Task<User> CreateAccount(string email, string password, string billingType = "demo")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/" + "users");
            
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var user = new User {Email = email, Password = password, BillingType = billingType};
            request.Content = new StringContent(
                Serialize(user),
                Encoding.UTF8, "application/json");


            var r = await client.SendAsync(request);
            var resp = Deserialize<Response<User>>(await r.Content.ReadAsStringAsync());


            CheckCode(resp, new List<int>{201});

            return resp.Data;
        }

        /// <summary>
        /// Request JWT by email & password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Access token</returns>
        public static async Task<string> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/" + "login");

            request.Content = new StringContent(
                Serialize(new {email, password}),
                Encoding.UTF8, "application/json");


            var r = await client.SendAsync(request);
            var resp = Deserialize<Response<Login>>(await r.Content.ReadAsStringAsync());

            CheckCode(resp, new List<int> { 200 });

            return resp.Data.AccessToken;
        }
    }
}