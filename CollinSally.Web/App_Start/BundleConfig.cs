using System.Web.Optimization;

namespace CollinSally.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Assets/Scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.form.js",
                        "~/Scripts/jquery.dynamiclist.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/CollinSally.js"));

            bundles.Add(new ScriptBundle("~/Assets/jQueryVal").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/CSS").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"));
        }
    }
}