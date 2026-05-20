namespace UserService;

public class InMemoryDatabase
{
    private int _nextId = 1;
    
    public List<User> Users { get; set; }
    
    public List<RefreshToken> RefreshTokens { get; set; } = [];

    public InMemoryDatabase()
    {
        Users = [
            new User(
                _nextId++,
                "Bartek",
                "bartus@email.com",
                "Password",
                ["admin", "user"]
            ),
            new User (
                _nextId++,
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
        return Users.First(user => user.Id == id);
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
            _nextId++,
            name,
            email,
            "",
            ["user"]
        );
        Users.Add(user);
        return user;
    }
}