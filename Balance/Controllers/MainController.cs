using Balance.Entities;
using Balance.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Balance.Controllers
{
    [Route("api/[controller]")]
    public class MainController : Controller
    {

        private BalanceService BalanceService { get; set; }

        public MainController(IOptions<List<Server>> serverList)
        {
            this.BalanceService = new BalanceService(serverList.Value);
        }

        [HttpGet]
        public string Get()
        {
            return this.BalanceService.GetAll();
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
