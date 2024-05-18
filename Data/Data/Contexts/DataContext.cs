using Data.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<SubscribeEntity> Subscribers { get; set; }
}
