using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.Configure<RazorViewEngineOptions>(options => {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
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
            app.UseMvc();
        }
    }


    public class ViewLocationExpander : IViewLocationExpander
    {

        /// <inheritdoc />
        /// <summary>
        /// Used to specify the locations that the view engine should search to 
        /// locate views.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewLocations"></param>
        /// <returns></returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //{2} is area, {1} is controller,{0} is the action
            string[] locations = { "/Views/{2}/{1}/{0}.cshtml", "/Pages/{0}.cshtml" };
            return locations.Union(viewLocations);          //Add mvc default locations after ours
        }


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }
    }
}