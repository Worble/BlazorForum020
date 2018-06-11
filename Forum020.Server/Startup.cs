// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Forum020.Data;
using Forum020.Domain.UnitOfWork;
using Forum020.Server.Services;
using Forum020.Server.Services.Interfaces;
using Forum020.Service.Interfaces;
using Forum020.Service.Services;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Mime;

namespace Forum020.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ForumContext>(options =>
                options.UseNpgsql(Configuration.GetValue<string>("ConnectionString"),
                    b => b.MigrationsAssembly("Forum020.Server")));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<string>("Redis");
                options.InstanceName = "RedisCache";
            });
            services.AddSession();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/event-stream",
                    MediaTypeNames.Application.Octet,
                    //WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                    );
            });

            //services.AddServerSentEvents();
            services.AddServerSentEvents<INotificationsServerSentEventsService, NotificationsServerSentEventsService>();
            services.AddNotificationsService(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
            });

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IBoardService, BoardService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IImageService, ImageService>();
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
                .MapServerSentEvents("/sse-notifications", serviceProvider.GetService<NotificationsServerSentEventsService>())
                .UseStaticFiles()
                .UseSession()
                .UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            //app.UseBlazor<Client.Program>();
        }
    }
}
