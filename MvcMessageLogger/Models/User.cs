namespace MvcMessageLogger.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public List<Message> Messages { get; } = new List<Message>();

        //public User(string name, string username)
        //{
        //    Name = name;
        //    Username = username;
        //}

        public User()
        {

        }
    }
}
