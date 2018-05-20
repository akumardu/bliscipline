using Bliscipline.Data.Repositories;
using Bliscipline.OAuth.Configuration;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Bliscipline.OAuth
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserValidator, UserValidator>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<Func<IDbConnection>>(() => new SqlConnection(Configuration.GetConnectionString("SocialNetwork")));

            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2(@"C:\Code\Pluralsight\Module2\SocialNetwork\socialnetwork.pfx", "password"))
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddConfigurationStore(options => {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("SocialNetwork.OAuth"), sql => sql.MigrationsAssembly(assembly));
                })
                .AddOperationalStore(options => {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("SocialNetwork.OAuth"), sql => sql.MigrationsAssembly(assembly));
                });


            services.AddMvc();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseGoogleAuthentication(new GoogleOptions
            {
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "777277052192-g9kis02f5sqg45ihea4o1ud3ma92d097.apps.googleusercontent.com",
                ClientSecret = "6bqtKHdfnr24Wpkuc-B2OInx"
            });

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
