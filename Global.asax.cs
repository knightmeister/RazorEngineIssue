using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Razor;
using System.Web.Routing;
using RazorEngine.Compilation;
using RazorEngine.Templating;
using WebApplication1.Models;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // init razor
            var razor = IsolatedRazorEngineService.Create(() => createRazorSandbox());

            // create the content model
            ContentModel cm = new ContentModel();
            cm.Html = "<h1>html here</h1>";
            cm.Title = "Title";
            var authors = new List<UserDetail>();
            authors.Add(new UserDetail() { EmailAddress = "hello@world.com", Name = "Test" });
            cm.History.Authors = authors;

            // run the transform
            string template = "@Model.Html";
            string result = razor.RunCompile(template, "name", typeof(ContentModel), RazorDynamicObject.Create(cm));

            
        }

        private AppDomain createRazorSandbox()
        {
            System.Security.Policy.Evidence ev = new System.Security.Policy.Evidence();
            ev.AddHostEvidence(new System.Security.Policy.Zone(SecurityZone.Internet));
            PermissionSet permSet = SecurityManager.GetStandardSandbox(ev);
            // We have to load ourself with full trust
            System.Security.Policy.StrongName razorEngineAssembly = typeof(RazorEngineService).Assembly.Evidence.GetHostEvidence<System.Security.Policy.StrongName>();
            // We have to load Razor with full trust (so all methods are SecurityCritical)
            // This is because we apply AllowPartiallyTrustedCallers to RazorEngine, because
            // We need the untrusted (transparent) code to be able to inherit TemplateBase.
            // Because in the normal environment/appdomain we run as full trust and the Razor assembly has no security attributes
            // it will be completely SecurityCritical. 
            // This means we have to mark a lot of our members SecurityCritical (which is fine).
            // However in the sandbox domain we have partial trust and because razor has no Security attributes that means the
            // code will be transparent (this is where we get a lot of exceptions, because we now have different security attributes)
            // To work around this we give Razor full trust in the sandbox as well.
            System.Security.Policy.StrongName razorAssembly = typeof(RazorTemplateEngine).Assembly.Evidence.GetHostEvidence<System.Security.Policy.StrongName>();
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, razorEngineAssembly, razorAssembly);

            return newDomain;
        }
    }
}
