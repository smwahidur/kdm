using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using KDM.Helpers;
using Serilog;
using System.IO;
using Hangfire;

[assembly: OwinStartup(typeof(KDM.Startup))]

namespace KDM
{
    public class Startup
    {
        public Startup()
        {
            KDMSettings.LoadSettings();
            
            string logPath = Path.Combine(KDMEnvironmentConstants.ApplicationPath,"logs");
            string filePath = Path.Combine(logPath, ".txt");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
        }

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureAuth(app);

            GlobalConfiguration.Configuration.UseSqlServerStorage("DBHANGFIRE");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            
            /*
            BackgroundJob.Schedule(
                () => Console.WriteLine("Hello, world"),
                TimeSpan.FromMinutes(1));

            BTreeHelpers pvbvProcessor = new BTreeHelpers();
            string jobID = "ProcessBinaryPVBV";
            RecurringJob.RemoveIfExists(jobID); */
            //RecurringJob.AddOrUpdate(jobID,() => pvbvProcessor.ProcessBinaryPVBV(), "*/5 * * * *"); 
                
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                ExpireTimeSpan = TimeSpan.FromMinutes(KDMEnvironmentConstants.SessionTimeout),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            

        }
    }
}
