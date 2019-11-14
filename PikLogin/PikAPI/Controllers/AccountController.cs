using PikAPI.Models;
using System.Web.Http;

namespace PikAPI.Controllers
{
    public class AccountController : ApiController
    {
        [HttpPost]
        [Route("api/Account/Save")]
        public Response Save([FromBody]Account account)
        {
            if (LoginManager.IsCorrectLogin(account.Login))
            {
                return Response.Ok(account);
            }
            else
            {
                return Response.Error("Пользователь с таким логином не найден");
            }
        }
    }
}
