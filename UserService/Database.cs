namespace UserService;

public class Database
{
    public List<User> Users { get; set; }

    public Database()
    {
        Users = [
            new User
            {
                Id = 0,
                Username = "Bartek",
                Password = "Password",
                Roles = ["admin", "user"]
            }
            ,
            new User
            {
                Id = 1,
                Username = "Aneta",
                Password = "haslo",
                Roles = ["user"]
            }
        ];
    }

    public User? GetUserLogin(string username, string password)
    {
        return Users.FirstOrDefault(user => user.Username == username && user.Password == password);
    }
    
    public User GetUserById(int id)
    {
        User user = Users.First(user => user.Id == id);
        user.Password = "";
        return user;
    }
    
    public List<User> GetUsers()
    {
        return Users;
    }
    
    public User DeleteUser(User user)
    {
        Users.Remove(user);
        return user;
    }
}