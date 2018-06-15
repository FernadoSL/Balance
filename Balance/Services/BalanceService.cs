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
        private HttpClientService Client { get; set; }

        private static List<Server> ServerList = new List<Server>();

        private static Stopwatch Timer = new Stopwatch();

        private static int LasIdServerUsed = 0;

        public BalanceService(List<Server> serverList)
        {
            this.Client = new HttpClientService();

            if (!ServerList.Any())
            {
                ServerList = serverList;
                Timer.Start();
            }

            PingAllServers();
        }

        public List<string> GetAll()
        {
            Server server = this.PickServer();
            return this.GetAll(server);
        }

        public List<string> GetAll(Server server)
        {
            return this.Client.GetAll(server);
        }

        public void Insert(string value)
        {
            Server server = this.PickServer();
            this.Client.Insert(value, server);
        }

        public void Update(string key, string value)
        {
            Server server = this.PickServer();
            this.Client.Update(key, value, server);
        }

        public void Delete(string key)
        {
            Server server = this.PickServer();
            this.Client.Delete(key, server);
        }

        public string GetByKey(string key)
        {
            Server server = this.PickServer();
            return this.Client.GetByKey(key, server);
        }

        private bool PingHost(Server server)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server.Ip);

                if (reply.Status == IPStatus.Success)
                {
                    return this.Client.HealthCheck(server);
                }

                return false;
            }
            catch (Exception)
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
                Server server = this.GetWorkingServer();

                if (server != null && server.Working)
                {
                    LasIdServerUsed = server.ServerId;
                    return server;
                }

                PingAllServers();
            }
        }

        private Server GetWorkingServer()
        {
            if (ServerList.Count(s => s.Working) > 1)
                return ServerList.FirstOrDefault(s => s.ServerId != LasIdServerUsed && s.Working);
            else
                return ServerList.FirstOrDefault(s => s.Working);
        }

        private void Update()
        {
            List<string> allData = new List<string>();

            foreach (var server in ServerList)
            {
                allData.AddRange(this.GetAll(server));
            }

            foreach (var server in ServerList)
            {
                this.Client.UpdateAllData(server, allData);
            }
        }
    }
}
