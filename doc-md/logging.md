* **Serilog for EF Core in ASP.NET Core**
    * [When ASP.NET Core Uses Default ILoggerFactory](#when-aspnet-core-uses-default-iloggerfactory)
    * [When ASP.NET Core Uses SerilogLoggerFactory](#when-aspnet-core-uses-serilogloggerfactory)
* [Logging EF Core with Default ILoggerFactory](#) (Independent of ASP.NET Core)

## When ASP.NET Core Uses Default ILoggerFactory
* Courtesy: [this](https://dejanstojanovic.net/aspnet/2018/october/logging-ef-core-actions-to-a-file-using-serilog/)
* ASP.NET Core uses Default ILoggerFactory for logging
* EF Core will use Serilog

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

## When ASP.NET Core Uses SerilogLoggerFactory
* Courtesy: [this](https://dennisroche.com/ef-core-logging-with-serilog/)
* ASP.NET Core uses [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore)
* Serilog.AspNetCore replaces the default ILoggerFactory and uses SerilogLoggerFactory for logging
* EF Core will also use Serilog

**Note:**   
The Serilog implementation of SerilogLoggerFactory is registered in the container, so when your configuring the EF Core DbContext inject the ILoggerFactory and pass it into options builder.

`BloggingContext.cs`
```cs
public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options, ILoggerFactory loggerFactory)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(_loggerFactory);
    }

    private readonly ILoggerFactory _loggerFactory;
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
