
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
            Client[] clients = [new Client{Name = "a"}, new Client{Name = "B"}];
           return clients;
        }
    }
}
