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

namespace Balance.Controllers
{
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        private static List<Server> ServerList = new List<Server>();

        private static Stopwatch Timer = new Stopwatch();

        private HttpClient Client { get; set; }

        public MainController(IOptions<List<Server>> serverList)
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

        [HttpGet]
        public string Get()
        {
            return "value";
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
