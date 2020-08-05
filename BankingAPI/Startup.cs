using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BankingAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using BankingAPI.DataAccess;
using BankingAPI.Models;

namespace BankingAPI
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                     {
                         options.RequireHttpsMetadata = false;
                         options.SaveToken = true;
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = Configuration["Jwt:Issuer"],
                          ValidAudience = Configuration["Jwt:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                          ClockSkew = TimeSpan.Zero
                         };
                         services.AddCors();
                    });
            services.AddAuthorization(Configuration =>
            {
                Configuration.AddPolicy(Policies.User, Policies.UserPolicy());
            });
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });
            services.Add(new ServiceDescriptor(typeof(BankingContext), new BankingContext(Configuration.GetConnectionString("mainDB"))));
            services.Add(new ServiceDescriptor(typeof(ReplicaContext), new ReplicaContext(Configuration.GetConnectionString("replicaDB"))));
            services.AddHttpClient("bankAPIClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => { return HttpUtils.HttpClientHandler; }).SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddMemoryCache();
            services.AddCors();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            //Add response headers
            app.Use(async (context, nextMiddleWhere) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    return Task.FromResult(0);
                });
                await nextMiddleWhere();
            });
           

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
