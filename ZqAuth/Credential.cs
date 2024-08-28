using ZqAuth.Base;

namespace ZqAuth;

public class Credential : BaseEntity
{
    public Credential(){}

    public Credential(string userId, string password, string domain)
    {
        UserId = userId;
        Password = password;
        Domain = domain;
    }
    
    public string UserId { get; set; }
    
    public string Password { get; set; }
    
    public string Domain { get; set; }

    public bool IsActive { get; set; } = true;
    
    public string? Secret { get; set; }
}