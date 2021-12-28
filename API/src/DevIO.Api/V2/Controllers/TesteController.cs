using DevIO.Api.Controllers;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:ApiVersion}/teste")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador, IUser appUser) : base(notificador, appUser) { }

        [HttpGet]
        public string Valor() => "Sou a V2";
    }
}
