using System.Web.Http;

namespace WebApp.Controllers
{
    public class DefaultController : ApiController
    {
        public IHttpActionResult GetProduct(int id)
        {
            return Ok(new { myId = id});
        }
    }
}
