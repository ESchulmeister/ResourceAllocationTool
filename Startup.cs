using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Services;
using System;

using Microsoft.EntityFrameworkCore;


namespace ResourceAllocationTool
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews()
             //   .AddRazorRuntimeCompilation()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            //antiforgery
            services.AddAntiforgery(options => options.HeaderName = Constants.AntiForgery.Header);

            //suppress log(log4net)  status messages
            services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);

            services.AddMemoryCache();

            services.AddRazorPages()
             .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            //database connection
            services.AddDbContext<TechCMSContext>(options =>
                                          options.UseSqlServer(Configuration.GetConnectionString("TechCMSDB")));

            //set up mappinggs for all endponts - ref. classes/mappings @ Profiles folder
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  //AD user context

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(
            CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
            {
                options.Cookie.Name = Constants.AuthCookie;
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            // configure scopes for application services:
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();

            //CORS  - enable cross-origin http requests
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
                app.UseHsts();
            }


            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

        }
    }
}
