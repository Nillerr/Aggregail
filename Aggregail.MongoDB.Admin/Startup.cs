using System;
using Aggregail.MongoDB.Admin.Authentication;
using Aggregail.MongoDB.Admin.Controllers;
using Aggregail.MongoDB.Admin.Documents;
using Aggregail.MongoDB.Admin.Helpers;
using Aggregail.MongoDB.Admin.Hubs;
using Aggregail.MongoDB.Admin.Services;
using Aggregail.MongoDB.Admin.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Starting Aggregail MongoDB Admin UI...");

            var settings = new AggregailMongoDBSettings();
            Configuration.Bind(settings);
            
            settings.Validate();
            services.AddSingleton(settings);
            
            services.AddControllersWithViews();
            services.AddSignalR();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            
            services.AddSingleton(s =>
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.Database);
                return database.GetCollection<RecordedEventDocument>(settings.Collection);
            });
            
            services.AddSingleton(s =>
            {
                var client = new MongoClient(settings.Users.ConnectionString ?? settings.ConnectionString);
                var database = client.GetDatabase(settings.Users.Database ?? settings.Database);
                return database.GetCollection<UserDocument>(settings.Users.Collection);
            });
            
            services.AddSingleton<UserDocumentPasswordHasher>();

            // TODO @nije: SignalR evaluation
            // services.AddHostedService<StreamService>();
            
            services.AddHostedService<UserBackgroundService>();

            services.AddSingleton<UserValidationEvents>();

            services.AddAuthorization();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "Authorization";
                    options.EventsType = typeof(UserValidationEvents);
                    options.TicketDataFormat = new JsonWebTokenDataFormat(new Microsoft.AspNetCore.Authentication.SystemClock());
                });

            if (!settings.QuietStartup)
            {
                var connectionString = MongoConnectionString.Censored(settings.ConnectionString);
                Console.WriteLine($"[Streams Configuration]");
                Console.WriteLine($" - MongoDB ConnectionString: {connectionString}");
                Console.WriteLine($"   - Database: {settings.Database}");
                Console.WriteLine($"   - Collection: {settings.Collection}");
                Console.WriteLine(string.Empty);

                var usersConnectionString =
                    MongoConnectionString.Censored(settings.Users.ConnectionString ?? settings.ConnectionString);
                Console.WriteLine($"[Users Configuration]");
                Console.WriteLine($" - MongoDB ConnectionString: {usersConnectionString}");
                Console.WriteLine($"   - Database: {settings.Users.Database ?? settings.Database}");
                Console.WriteLine($"   - Collection: {settings.Users.Collection}");
                Console.WriteLine(string.Empty);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCookiePolicy(new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.Strict,
                    HttpOnly = HttpOnlyPolicy.Always,
                    Secure = CookieSecurePolicy.SameAsRequest,
                }
            );

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}"
                    );
                 
                    // TODO @nije: Evaluate whether adding SignalR is fun or not
                    // endpoints.MapHub<StreamHub>("hubs/stream");
                }
            );

            app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                }
            );
        }
    }
}