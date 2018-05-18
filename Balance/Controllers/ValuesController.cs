using Balance.Configuration;
using Balance.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Timers;

namespace Balance.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static List<Server> ServerList = new List<Server>();

        private static Stopwatch Timer = new Stopwatch();

        private HttpClient Client { get; set; }

        public ValuesController(IOptions<List<Server>> serverList)
        {
            this.Client = new HttpClient();
            
            if (!ServerList.Any())
            {
                ServerList = serverList.Value;
                Timer.Start();
            }

            foreach (var server in ServerList)
            {
                server.Working = this.PingHost(server);
                if (Timer.ElapsedMilliseconds > 60000)
                {
                    this.Update();
                    Timer.Restart();
                }
            }

        }

        // GET api/values
        [HttpGet]
        public List<Server> Get()
        {
            return ServerList;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool PingHost(Server server)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server.Ip);

                if (reply.Status == IPStatus.Success)
                {
                    string url = string.Format("http://{0}:{1}", server.Ip, server.PortNumber);

                    var response = this.Client.GetAsync(url).Result;
                    return response.StatusCode == HttpStatusCode.OK;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Update()
        {
            foreach (var server in ServerList)
            {
                if (server.Working)
                {
                    string url = string.Format("http://{0}:{1}/{2}", server.Ip, server.PortNumber, "getAllKeys");
                    var response = this.Client.GetAsync(url).Result;
                }
            }

            foreach (var server in ServerList)
            {
                if (server.Working)
                {
                    string url = string.Format("http://{0}:{1}/{2}", server.Ip, server.PortNumber, "update");
                    var response = this.Client.GetAsync(url).Result;
                }
            }
        }
    }
}
