## Logging with ILoggerFactory
* EF Core 2.2+

`FooDbContext.cs`
```cs
public class FooDbContext : DbContext
{
    public FooContext(DbContextOptions<FooDbContext> options) : base(options) { }

    public DbSet<Foo> Foos { get; set; }
    public DbSet<Bar> bars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // ...
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = FooAppData; Trusted_Connection = True; ");
        optionsBuilder.UseLoggerFactory(GetLoggerFactory());
    }
    
    private ILoggerFactory GetLoggerFactory()
    {
      IServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddLogging(builder => builder.AddConsole().AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information));
      return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
    }
}
```
