
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreModel.Clients;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace core1.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpGet]
        public Client[] Get(){
            Client[] clients = [new Client{Id="12345678", Name = "Company a"}, new Client{Id="22222222", Name = "Company B"}];
           return clients;
        }
    }
}
