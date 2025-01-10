namespace Synchronizator.Data;

public class ApplicationUserHasFile
{
    public Guid Id { get; set; }
    
    public string DiskPath { get; set; }
    
    public string Token { get; set; }
    
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}