using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UserPermissionsN5.Data;
using UserPermissionsN5.Models;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
            typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                if (!context.PermissionTypes.Any())
                {
                    context.PermissionTypes.Add(new PermissionType
                    {
                        Id = 1,
                        Description = "Vacation"
                    });

                    context.SaveChanges();
                }
                    if (!context.Permission.Any())
                    {
                        context.Permission.Add(new Permission
                        {
                            EmployeeForeName = "Jhon",
                            EmployeeSurName = "Mendez",
                            PermissionTypeId = 1,  
                            PermissionDate = DateTime.Now
                        });

                        context.SaveChanges();
                    }
            }
        });
    }
}
