using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ontap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ontap.Auth;
using Elmah.Io.AspNetCore;
using Ontap.Util;

namespace Ontap
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var sqlConnectionOptions = Configuration.GetSection(nameof(SqlConnectionOptions));
            services.AddDbContext<DataContext>(options 
                =>
            {
                if (!Enum.TryParse(
                    Configuration.GetValue<string>("SqlConnectionOptions:ConnectionType"),
                    out SqlConnectionOptions.ConnectionTypes connectionType))
                {
                    connectionType = SqlConnectionOptions.ConnectionTypes.Sqlserver;
                }

                var connectionName = Configuration.GetValue<string>("SqlConnectionOptions:ConnectionString");
                var connectionString = Configuration.GetConnectionString(connectionName);
                if (connectionType == SqlConnectionOptions.ConnectionTypes.Mysql)
                {
                    var databaseUrl = Configuration.GetValue<string>("DATABASE_URL");
                    if (Utilities.TryParseDatabaseUrl(databaseUrl, out var s))
                    {
                        connectionString = s;
                    }
                    options.UseMySql(connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
                //options.UseSqlServer(Configuration.GetValue<string>("SQLSERVER_CONNECTION_STRING")
                //                     ?? Configuration.GetConnectionString("DefaultConnection"));
            });
            // Add framework services.
            services.AddCors();
            services.AddMvc(config =>
            {
                // Make authentication compulsory across the board (i.e. shut
                // down EVERYTHING unless explicitly opened up).
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                //config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.ValidFor = TimeSpan.Parse(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)]);
                options.SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminUser", policy => policy.RequireUserType("Admin"));
                options.AddPolicy("PubAdminUser", policy => policy.RequireUserType("Admin", "PubAdmin"));
                options.AddPolicy("BreweryAdminUser", policy => policy.RequireUserType("Admin", "BrewerAdmin"));
                options.AddPolicy("BreweryOrPubAdminUser",
                    policy => policy.RequireUserType("Admin", "PubAdmin", "BrewerAdmin"));
            });

        }

        private SymmetricSecurityKey SigningKey
        {
            get { return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"))); }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //loggerFactory.CreateLogger("MyLog");
            //loggerFactory.AddLoggr(LogLevel.Error, "ysilvestrov", "7b35847110724b9fba13359525810c9a");

            var apiKey = Configuration.GetValue<string>("ELMAH_API");
            var apiGuid = Configuration.GetValue<string>("ELMAH_LOG");

            if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiGuid))
                app.UseElmahIo(apiKey, new Guid(apiGuid));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            ConfigureDb(app);

            ConfigureJwt(app);

            app.UseStaticFiles();

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            app.UseMiddleware(typeof(JsonErrorHandlingMiddleware));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

        }

        private void ConfigureDb(IApplicationBuilder app)
        {
            using (
                var serviceScope =
                    app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<DataContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<DataContext>().EnsureSeedData(Configuration);
            }
        }

        private void ConfigureJwt(IApplicationBuilder app)
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
        }
    }
}
