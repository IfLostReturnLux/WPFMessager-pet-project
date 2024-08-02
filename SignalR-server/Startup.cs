using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;

namespace SignalR
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            //services.AddControllers()
            //     .AddNewtonsoftJson(
            //          options => {
            //              options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //          });
            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddControllersWithViews();
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<FriendHub>("/friendHub");
                endpoints.MapGet("/", async (context) =>  await context.Response.WriteAsync(File.ReadAllText(@"C:\Users\suvor\source\repos\SignalR\wwwroot\view\Main.html")));
            });
        }
    }
}
