namespace UserService;

public class LoggedUsers
{
    private static List<int> loggedIds = [];

    public void RegisterLogin(User user)
    {
        loggedIds.Add(user.Id);
    }

    public void RegisterLogout(int id)
    {
        int result = loggedIds.RemoveAll(i => i == id);

        if (result != 1)
        {
            throw new Exception("Too much data deleted");
        }
    }
    
    public int GetStatus()
    {
        return loggedIds.Count;
    }
}