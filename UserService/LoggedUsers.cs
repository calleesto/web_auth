namespace UserService;

public class LoggedUsers
{
    public List<int> LoggedIds { get; set; }

    public LoggedUsers()
    {
        LoggedIds = [];
    }

    public void RegisterLogin(User user)
    {
        LoggedIds.Add(user.Id);
    }

    public void RegisterLogout(User user)
    {
        int result = LoggedIds.RemoveAll(id => id == user.Id);

        if (result != 1)
        {
            throw new Exception("Too much data deleted");
        }
    }
    
    public int GetStatus()
    {
        return LoggedIds.Count;
    }
    
    
}