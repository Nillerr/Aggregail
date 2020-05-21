using System.Text.Json;
using Aggregail.MongoDB.Admin.Authentication;
using Aggregail.MongoDB.Admin.Controllers;
using Aggregail.MongoDB.Admin.Documents;
using Aggregail.MongoDB.Admin.Hubs;
using Aggregail.MongoDB.Admin.Services;
using Aggregail.MongoDB.Admin.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllersWithViews();
            services.AddSignalR();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            var aggregailSettings = new AggregailSettings();
            Configuration.Bind("Aggregail", aggregailSettings);
            
            services.AddSingleton(aggregailSettings);

            services.AddSingleton<MongoDatabaseFactory>();
            services.AddSingleton<MongoCollectionFactory>();

            services.AddSingleton<RecordedEventCollectionFactory>();
            services.AddSingleton(s => s.GetRequiredService<RecordedEventCollectionFactory>().Collection);
            services.AddSingleton(s => s.GetRequiredService<MongoCollectionFactory>().Collection<UserDocument>("users"));
            
            services.AddSingleton<UserDocumentPasswordHasher>();

            services.AddHostedService<StreamService>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.Strict,
                    HttpOnly = HttpOnlyPolicy.Always,
                    Secure = env.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always,
                }
            );

            app.UseHttpsRedirection();
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
                    
                    endpoints.MapHub<StreamHub>("hubs/stream");
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