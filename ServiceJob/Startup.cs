using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceJob.Util;

namespace ServiceJob
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
            var mvc = services.AddMvc();
            mvc.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            mvc.AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Home",
                    @""); // route Index = RazorPage "Home.cshtml" (default search RazorPage = "Pages/")
            });
            //services.Configure<MvcViewOptions>(options =>
            //{
            //    options.ViewEngines.Insert(options.ViewEngines.Count, new CustomViewEngine());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Views/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "jvnlp",
                //    template: "/Jvnlp",
                //    defaults: new { controller = "Views", action = "Jvnlp" }
                //    );
            });
        }
    }
}