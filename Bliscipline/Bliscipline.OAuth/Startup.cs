using Bliscipline.Data.Repositories;
using Bliscipline.OAuth.Configuration;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
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
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Bliscipline.OAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserValidator, UserValidator>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<Func<IDbConnection>>(() => new SqlConnection(Configuration.GetConnectionString("bliscipline")));

            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2(@"C:\Users\amdubedy\Documents\Learn\bliscipline\Bliscipline\bliscipline.pfx", "password"))
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddConfigurationStore(options => {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("bliscipline.OAuth"), sql => sql.MigrationsAssembly(assembly));
                })
                .AddOperationalStore(options => {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("bliscipline.OAuth"), sql => sql.MigrationsAssembly(assembly));
                });

            services.AddAuthentication().AddGoogle(options => 
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = "406959995629-bbn2oi4ckjmptvs64dok2c3in60hq4oi.apps.googleusercontent.com";
                options.ClientSecret = "rZygr2EbaG7aY4g9AR8Z3FSa";
            });
            services.AddMvc();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            MigrateInMemoryDataToSqlServer(app);

            loggerFactory.AddConsole();

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "777277052192-g9kis02f5sqg45ihea4o1ud3ma92d097.apps.googleusercontent.com",
            //    ClientSecret = "6bqtKHdfnr24Wpkuc-B2OInx"
            //});

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

        public void MigrateInMemoryDataToSqlServer(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in InMemoryConfiguration.Clients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in InMemoryConfiguration.IdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in InMemoryConfiguration.ApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
