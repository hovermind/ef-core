using System;
using System.IO;
using System.Linq;
using DbFirst.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Blog Url List:\n");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("SqlExpress");

            var bloggingContext = new BloggingContext(connectionString);

            var blogs = bloggingContext.Blogs.ToList();
            foreach (var b in blogs)
            {
                Console.WriteLine(b.Url);
            }

            Console.ReadKey();
        }
    }
}
