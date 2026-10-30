public class User : IEntity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }

    public User(string username, string email, bool isActive)
    {
        Username = username;
        Email = email;
        IsActive = isActive;
    }

    public override string ToString()
    {
        return $"[{Id}] {Username} <{Email}> активний: {(IsActive ? "так" : "ні")}";
    }
}