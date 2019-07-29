using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace ServiceJob.Util
{
    public class CustomViewEngine : IViewEngine
    {
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            var path = $"Pages/{viewName}.html";
            if (File.Exists(path))
                return ViewEngineResult.Found(viewName, new NullView());
            return ViewEngineResult.NotFound(path, new[] {"/" + path});
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            var path = $"Pages/{viewPath}.cshtml";
            if (File.Exists(path))
                return ViewEngineResult.Found(viewPath, new CustomView(path));
            return ViewEngineResult.NotFound(path, new[] {"/" + path});
        }
    }

    public class CustomView : IView
    {
        public CustomView(string viewPath)
        {
            Path = viewPath;
        }

        public string Path { get; set; }

        public async Task RenderAsync(ViewContext context)
        {
            //context.Writer.WriteAsync(content);
            await context.ViewBag(Path);
        }
    }
}