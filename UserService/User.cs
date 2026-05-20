namespace UserService;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public List<string> Roles { get; set; }

    public User(int id, string name, string email, string password, List<string> roles)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = BCrypt.Net.BCrypt.HashPassword(password);
        Roles = roles;
    }
}