namespace UserService;

public class Database
{
    public List<User> Users { get; set; }

    public Database()
    {
        Users = [
            new User(
                "Bartek",
                "bartus@email.com",
                "Password",
                ["admin", "user"]
            ),
            new User (
                "Aneta",
                "anetka@email.com",
                "haslo",
                ["user"]
            )
        ];
    }

    public User? GetUserLogin(string username, string password)
    {
        User? user = Users.FirstOrDefault(user =>
                user.Name == username &&
                BCrypt.Net.BCrypt.Verify(password, user.Password)
            );
        
        return user;
    }
    
    public User GetUserById(int id)
    {
        User user = Users.First(user => user.Id == id);
        user.Password = "";
        return user;
    }

    public User? GetUserByEmail(string email)
    {
        return Users.FirstOrDefault(user => user.Email == email);
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

    public User CreateGoogleUser(string name, string email)
    {
        User user = new (
            name,
            email,
            "",
            ["user"]
        );
        Users.Add(user);
        return user;
    }
}