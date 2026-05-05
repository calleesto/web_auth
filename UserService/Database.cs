namespace UserService;

public class Database
{
    public List<User> Users { get; set; }

    public Database()
    {
        Users = [];
    }

    public User GetUser(string username, string password)
    {
        return Users.First(user => user.Username == username && user.Password == password);
    }
}