using AutoMapper;
using BCM.Api.Configurations;
using BCM.Common.TokenHelper;
using BCM.Filters;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using BCM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace BCM.Api
{
    public class Startup
    {
        public static IConfiguration Configuration;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    // builder.AllowAnyOrigin() //允许任何来源的主机访问
                    builder.WithOrigins("http://localhost:8080") ////允许http://localhost:8080的主机访问
                    .WithOrigins("http://10.60.188.90:8080")
                    .WithOrigins("http://10.188.2.90:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddInnerAuthorize(Configuration.GetSection("JwtOption"));
            services.AddDbContext<IUnitOfWork, BCMContext>(o => o.UseSqlServer(Configuration.GetConnectionString("sqlseverconnectionString"), b => b.UseRowNumberForPaging()));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddRepositories();
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(LogAttribute));
                option.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseStatusCodePages();
            app.UseAuthentication();//启用验证
            // app.UseMiddleware(typeof(ExceptionHandlerMiddleware));
            app.UseMvc();

        }
    }
}
