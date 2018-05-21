using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Bliscipline.Web
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
            services.AddMvc();
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddOpenIdConnect(options => {
                    options.SignInScheme = "Cookies";
                    options.Authority = "http://localhost:59418";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "bliscipline_code";
                    options.ClientSecret = "secret";
                    options.ResponseType = "id_token code";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationScheme = "Cookies"
            //});

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    AuthenticationScheme = "oidc",
            //    SignInScheme = "Cookies",
            //    Authority = "http://localhost:59418",
            //    RequireHttpsMetadata = false,
            //    ClientId = "bliscipline_code",
            //    ClientSecret = "secret",
            //    ResponseType = "id_token code",
            //    Scope = { "bliscipline", "offline_access", "email" },
            //    GetClaimsFromUserInfoEndpoint = true,
            //    SaveTokens = true
            //});

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
