﻿using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;
using TestingControllersSample.Infrastructure;

namespace TestingControllersSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase());

            services.AddMvc();
            services.AddScoped<IBrainStormSessionRepository, EfStormSessionRepository>();
        }

        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Verbose);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseRuntimeInfoPage(); // default path is /runtimeinfo
                
                InitializeDatabase(app.ApplicationServices.GetService<AppDbContext>());

            }
            app.UseIISPlatformHandler();
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
        }

        public void InitializeDatabase(AppDbContext dbContext)
        {
            if (!dbContext.BrainStormSessions.Any())
            {
                var session = new BrainStormSession()
                {
                    Name = "Test Session 1",
                    DateCreated = new DateTime(2016, 8, 1)
                };
                var idea = new Idea()
                {
                    DateCreated = new DateTime(2016, 8, 1),
                    Description = "Totally awesome idea",
                    Name = "Awesome idea"
                };
                session.AddIdea(idea);
                dbContext.BrainStormSessions.Add(session);
                dbContext.SaveChanges();
            }
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
