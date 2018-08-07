// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Forum020.Data;
using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using Forum020.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace Forum020.Server
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //dbcontext
            services.AddDbContext<ForumContext>(options =>
                options.UseNpgsql(_config.GetValue<string>("ConnectionString"),
                    b => b.MigrationsAssembly("Forum020.Server")));

            //redis cache
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = _config.GetValue<string>("Redis");
                options.InstanceName = "RedisCache";
            });

            //json serializer
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //response compression
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/event-stream",
                    MediaTypeNames.Application.Octet,
                });
            });

            //cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.WithOrigins("http://localhost:40303")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(1))
                        );
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            //jwt auth
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = "worble.xyz",
            //            ValidAudience = "worble.xyz",
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                Encoding.UTF8.GetBytes(_config["SecurityKey"]))
            //        };
            //    });

            //mvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
            });

            //di
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IBoardService, BoardService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (!serviceScope.ServiceProvider.GetService<ForumContext>().AllMigrationsApplied())
                {
                    serviceScope.ServiceProvider.GetService<ForumContext>().Database.Migrate();
                }
                serviceScope.ServiceProvider.GetService<ForumContext>().EnsureSeeded();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression()
                .UseStaticFiles()
                .UseCors()
                .UseAuthentication()
                .UseCookiePolicy()
                .UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });
        }
    }
}
