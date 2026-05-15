namespace UserService;

public class LoggedUsers
{
    private static HashSet<int> loggedIds = [];

    public void RegisterLogin(User user)
    {
        loggedIds.Add(user.Id);
    }

    public void RegisterLogout(int id)
    {
        loggedIds.Remove(id);
    }
    
    public int GetStatus()
    {
        return loggedIds.Count;
    }
}