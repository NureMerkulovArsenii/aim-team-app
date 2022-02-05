using System;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DAL;

public class AppContext : DbContext
{
    private readonly AppSettings _appSettings;
    
    public AppContext(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
    }
    
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseSqlServer(_appSettings.ConnectionString);
    }
}
