using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Balance.Entities;

namespace Balance.Services
{
    public class HttpClientService
    {
        private static int LastKeyInserted = 0;

        private HttpClient Client { get; set; }

        public HttpClientService()
        {
            this.Client = new HttpClient();
        }

        public List<string> GetAll(Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "ReadAll");
            var response = this.Client.GetAsync(url).Result.Content.ReadAsAsync<List<string>>();

            return response.Result;
        }

        public void Insert(string value, Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "Insert");

            string key = (LastKeyInserted + 1).ToString();
            string data = string.Format("{0}:{1}", key, value);

            var response = Client.PostAsJsonAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
                LastKeyInserted++;
        }

        public void Update(string key, string value, Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);
            Client.PutAsJsonAsync(url, value);
        }

        public void Delete(string key, Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);
            Client.DeleteAsync(url);
        }

        public string GetByKey(string key, Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/" + key);
            var result = Client.GetAsync(url).Result;

            return result.Content.ReadAsStringAsync().Result;
        }

        public bool HealthCheck(Server server)
        {
            string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "HealthCheck");
            var response = this.Client.GetAsync(url).Result;

            return response.StatusCode == HttpStatusCode.OK;
        }

        public void UpdateAllData(Server server, List<string> allData)
        {
            if (server.Working)
            {
                string url = this.CreateUrl(server.Ip, server.PortNumber.ToString(), "api/values/UpdateAll");
                this.Client.PostAsJsonAsync(url, allData);
            }
        }

        private string CreateUrl(string ip, string portNumber, string mehtod)
        {
            return string.Format("http://{0}:{1}/{2}", ip, portNumber, mehtod);
        }
    }
}