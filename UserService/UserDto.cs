namespace UserService;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
    }
}