﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LykkePay.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LykkePay.API
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

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            BuildContainer(services);
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.Use(next => context => { context.Request.EnableRewind(); return next(context); });
            app.UseDeveloperExceptionPage();

            app.UseMvc();
        }

        private void BuildContainer(IServiceCollection services)
        {
            // var connectionString = Configuration.GetValue<string>("ConnectionString");

            // var log = new LogToTable(new AzureTableStorage<LogEntity>(connectionString, "Logs", null));

            //services.AddSingleton<ILog>(log);
            
            services.AddSingleton(Configuration);
            services.AddSingleton(new SecurityHelper());
            
        }
    }
}
