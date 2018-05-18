namespace Balance.Entities
{
    public class Server
    {
        public int ServerId { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public int PortNumber { get; set; }

        public bool Working { get; set; }

        public bool Updated { get; set; }
    }
}
