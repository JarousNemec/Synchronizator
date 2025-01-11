using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Synchronizator.Database.Tables;

namespace Synchronizator.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ApplicationUserHasToken> Tokens { get; set; }
    public DbSet<ApplicationUserHasFile> Files { get; set; }
}