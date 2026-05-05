namespace UserService;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; } // Never store plain-text passwords like this in production
    
    public string Role { get; set; }
}