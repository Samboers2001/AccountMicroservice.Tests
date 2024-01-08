using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AccountMicroservice.Data; // Adjust this using statement to match your actual namespace

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing context configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AppDbContext>)); // Replace ApplicationDbContext with your actual DbContext

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    Utilities.InitializeDbForTests(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                }
            }
        });
    }
}
