using Balance.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace Balance.Services
{
    public class BalanceService
    {
        private static List<Server> ServerList = new List<Server>();

        private static Stopwatch Timer = new Stopwatch();

        private HttpClient Client { get; set; }

        public BalanceService(List<Server> serverList)
        {
            this.Client = new HttpClient();

            if (!ServerList.Any())
            {
                ServerList = serverList;
                Timer.Start();
            }

            PingAllServers();
        }
        
        public string GetAll()
        {
            Server server = this.PickServer();
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "ReadAll");
            
            var result = Client.GetAsync(url).Result;

            return result.Content.ReadAsStringAsync().Result;
        }

        public void Insert(string value)
        {
            Server server = this.PickServer();
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "Insert");
            
            Client.PostAsJsonAsync(url, value);
        }

        public void Update(string key, string value)
        {
            Server server = this.PickServer();
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);

            Client.PutAsJsonAsync(url, value);
        }

        public void Delete(string key)
        {
            Server server = this.PickServer();
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);

            Client.DeleteAsync(url);
        }

        public string GetByKey(string key)
        {
            Server server = this.PickServer();
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);

            var result = Client.GetAsync(url).Result;

            return result.Content.ReadAsStringAsync().Result;
        }
        
        private bool PingHost(Server server)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server.Ip);

                if (reply.Status == IPStatus.Success)
                {
                    string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "HealthCheck");

                    var response = this.Client.GetAsync(url).Result;
                    return response.StatusCode == HttpStatusCode.OK;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void PingAllServers()
        {
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

        private Server PickServer()
        {
            while (true)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(ServerList.Count);

                Server server = ServerList.ElementAt(randomIndex);
                if (server.Working)
                    return server;

                if (Timer.ElapsedMilliseconds > 60000 && ServerList.All(s => !s.Working))
                    PingAllServers();
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

        private string CreateUrl(string ip, string portNumber, string mehtod)
        {
            return string.Format("http://{0}:{1}/{2}", ip, portNumber, mehtod);
        }
    }
}
