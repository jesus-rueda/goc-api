using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goc.Api.Controllers
{


    


    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ActionsController: ControllerBase
    {


        public ActionsController()
        {
                
        }



        
    }
}
