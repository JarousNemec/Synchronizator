namespace Synchronizator.Database.Tables;

public class ApplicationUserHasToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}