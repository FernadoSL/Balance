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

        private HttpClientService HttpClientService { get; set; }

        private Server Server { get { return this.BalanceService.PickServer(); } }

        public MainController(IOptions<List<Server>> serverList)
        {
            this.BalanceService = new BalanceService(serverList.Value);
            this.HttpClientService = new HttpClientService();
        }

        [HttpGet]
        public List<string> Get()
        {
            return this.HttpClientService.GetAll(this.Server);
        }

        [HttpGet("{key}")]
        public string Get(string key)
        {
            return this.HttpClientService.GetByKey(key, this.Server);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
            this.HttpClientService.Insert(value, this.Server);
        }

        [HttpPut("{key}")]
        public void Put(string key, [FromBody]string value)
        {
            this.HttpClientService.Update(key, value, this.Server);
        }

        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            this.HttpClientService.Delete(key, this.Server);
        }
    }
}
