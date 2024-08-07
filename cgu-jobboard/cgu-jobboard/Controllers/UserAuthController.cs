using cgu_jobboard.Models.Class;
using cgu_jobboard.Models.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace cgu_jobboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController : Controller
    {
        private readonly IHttpClientFactory? _httpClientFactory;

        public UserAuthController(IHttpClientFactory? httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("SignIn")]
        [EnableCors("default")]
        public async Task<IActionResult> SignIn([FromBody] UserLoginModel dataObject)
        {

            var email = dataObject.Email;
            var pwd = dataObject.Password;

            IEnumerable<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                new("email", email),
                new("password", pwd)
            };

            // create client and post 
            var apiClient = _httpClientFactory.CreateClient("api-gateway");

            var result = apiClient.PostAsync("gateway/User/login", new FormUrlEncodedContent(content)).Result;

            if (result.IsSuccessStatusCode)
            {
                var data = await result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserLoginDataModel>(data);

                Globals.UserId = user.Id;
                Globals.CompanyUser = user.CompanyUser;
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("CompanyUser", Convert.ToInt32(user.CompanyUser));

                return Json(new
                {
                    user = user.Id,
                    email = user.Email,
                    companyUser = user.CompanyUser
                });
            }
            else
            {
                return Json(null);
            }
        }
    }
}
