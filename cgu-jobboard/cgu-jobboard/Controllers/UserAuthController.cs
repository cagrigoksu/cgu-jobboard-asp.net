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

        [HttpPost("LogIn")]
        [EnableCors("default")]
        public async Task<IActionResult> LogIn([FromBody] UserLoginModel dataObject)
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
                Globals.CompanyUser = user.IsCompanyUser;
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("CompanyUser", Convert.ToInt32(user.IsCompanyUser));

                var resultPost = apiClient.GetAsync("gateway/JobPost/GetAllJobPostsByPage?page=1").Result;
                var dataPost = await resultPost.Content.ReadAsStringAsync();
                return Json(new
                {
                    user = user.Id,
                    email = user.Email,
                    companyUser = user.IsCompanyUser,
                    list = dataPost
                });
            }

            return Json(null);
        }

        [HttpPost("LogOn")]
        [EnableCors("default")]
        public async Task<IActionResult> LogOn([FromBody] UserLogonModel dataObject)
        {

            var email = dataObject.Email;
            var pwd = dataObject.Password;
            var pwdConf = dataObject.PasswordConfirmation;
            var isCompanyUser = dataObject.IsCompanyUser;

            IEnumerable<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                new("email", email),
                new("password", pwd),
                new("passwordconf", pwdConf),
                new("companyUser", isCompanyUser.ToString())
            };

            // create client and post
            var apiClient = _httpClientFactory.CreateClient("api-gateway");
            var result = apiClient.PostAsync("gateway/User/logon", new FormUrlEncodedContent(content)).Result;

            if (result.IsSuccessStatusCode)
            {
                var data = await result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserLoginDataModel>(data);

                Globals.UserId = user.Id;
                Globals.CompanyUser = user.IsCompanyUser;
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("CompanyUser", Convert.ToInt32(user.IsCompanyUser));

                return Json(new
                {
                    user = user.Id,
                    email = user.Email,
                    companyUser = user.IsCompanyUser
                });
            }

            return Json(null);
        }
    }
}
