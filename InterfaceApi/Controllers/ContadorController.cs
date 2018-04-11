using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansDotNet.GraosInterfaces;

namespace InterfaceApi.Controllers
{
    [Route("api/[controller]")]
    public class ContadorController : Controller
    {
        private IClusterClient client;

        public ContadorController(IClusterClient client){
            this.client = client;
        }
        
        [HttpGet]
        public async Task<int> Get()
        {
            var contador = client.GetGrain<IGraoContador>("TW-1");

            return await contador.GetContador();
        }


        [HttpPost]
        public async Task Post()
        {
            var contador = client.GetGrain<IGraoContador>("TW-1");
            await contador.Incremento(1);
        }
    }
}
