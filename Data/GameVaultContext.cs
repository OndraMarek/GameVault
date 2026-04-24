using GameVault.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data;

public class GameVaultContext : DbContext
{
    public DbSet<OwnedGame> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=gamevault.db");
    }
}