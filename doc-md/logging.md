## Serilog for EF Core in ASP.NET Core

#### When ASP.NET Core Uses Default ILoggerFactory
Courtesy: [this](https://dejanstojanovic.net/aspnet/2018/october/logging-ef-core-actions-to-a-file-using-serilog/)   

**1. Dependency**
```ps1
PM> Install-Package Serilog.Extensions.Logging.File
PM> Install-Package Serilog.Extensions.Logging.File
PM> Install-Package Serilog.Settings.Configuration
```

**2. `Startup.cs`** 
```cs
public class Startup  
{  
	public IConfiguration Configuration { get; private set; }  
	public IHostingEnvironment HostingEnvironment { get; private set; }  

	public Startup(IConfiguration configuration, IHostingEnvironment env)  
	{  
		Configuration = configuration;  
		HostingEnvironment = env;  
		Log.Logger = new LoggerConfiguration()  
							.ReadFrom.Configuration(configuration)  
							.CreateLogger();  

	}      

	public void ConfigureServices(IServiceCollection services)  
	{  
		services.AddLogging();  
	}  

	public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)  
	{  
	   loggerFactory.AddSerilog();  

	   app.UseMvc();  
	}  
}
```

**3. `appsettings.json`**
```json
{  
  "Serilog": {  
    "MinimumLevel": "Debug",  
    "WriteTo": [  
      {  
        "Name": "RollingFile",  
        "Args": {  
          "logDirectory": ".\\Logs",  
          "fileSizeLimitBytes": 104857600 ,  
          "pathFormat": "Logs/MyApplication.{Date}.log",  
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"  
        }  
      }  
    ]  
  }  
} 
```

**4. DbContext logging configuration**
`MySampleDbContext.cs`
```cs
public class MySampleDbContext : DbContext  
{  
    private readonly ILoggerFactory _loggerFactory;  
  
    public MySampleDbContext(DbContextOptions<BloggingContext> options, ILoggerFactory loggerFactory)  
        : base(options)  
    {  
        this.loggerFactory = loggerFactory;  
    }  
      
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  
    {  
        optionsBuilder.UseLoggerFactory(this.loggerFactory);  
    }  
}
```

## Logging with Default ILoggerFactory
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
