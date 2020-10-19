using System.Web;
using System.Web.Optimization;

namespace KDM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));


            bundles.Add(new Bundle("~/style/v1")
                .Include("~/Content/ArchitectUI/main.css")
                .Include("~/Content/Custom.css")
                .Include("~/Content/print.css")
                .Include("~/Scripts/daterangepicker/daterangepicker.css")
                .Include("~/Scripts/jquery-timepicker-1.3.5/jquery.timepicker.min.css")
                .Include("~/Scripts/Treant/Treant.css")
                .Include("~/Content/DataTables/datatables.min.css")
                );


            bundles.Add(new Bundle("~/script/v1")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Content/ArchitectUI/assets/scripts/main.js")
                .Include("~/Scripts/Custom.js")
                .Include("~/Scripts/daterangepicker/moment.min.js")
                .Include("~/Scripts/daterangepicker/daterangepicker.js")
                .Include("~/Scripts/jquery-timepicker-1.3.5/jquery.timepicker.min.js")
                .Include("~/Scripts/Treant/vendor/raphael.js")
                .Include("~/Scripts/Treant/Treant.js")
                .Include("~/Content/DataTables/datatables.min.js")
                );

            //BundleTable.EnableOptimizations = true;
        }
    }
}
