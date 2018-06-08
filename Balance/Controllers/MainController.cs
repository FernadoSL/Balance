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

        [HttpGet("{key}")]
        public string Get(string key)
        {
            return this.BalanceService.GetByKey(key);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
            this.BalanceService.Insert(value);
        }

        [HttpPut("{key}")]
        public void Put(string key, [FromBody]string value)
        {
            this.BalanceService.Update(key, value);
        }

        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            this.BalanceService.Delete(key);
        }
    }
}
