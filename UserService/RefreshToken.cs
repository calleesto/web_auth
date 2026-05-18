namespace UserService;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;

    public int UserId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }
}