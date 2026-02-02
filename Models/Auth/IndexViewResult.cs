public class IndexViewResult
{
    public bool IsLoggedIn { get; set; }
    public UserDetail? CurrentUser { get; set; }
    public List<UserList> Users { get; set; } = new List<UserList>();
}