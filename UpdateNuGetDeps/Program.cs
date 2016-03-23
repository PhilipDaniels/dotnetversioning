using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace UpdateNuGetDeps
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args[0] == "--help" || args[0] == "?" || args[0] == "-h")
                ShowHelpAndExit();

            var sw = new Stopwatch();
            sw.Start();

            int numUpdated = 0;
            if (args[0] == "-r")
            {
                // Find an update everything. The best and easiest way!
                var nuspecs = Directory.GetFiles(Environment.CurrentDirectory, "*.nuspec", SearchOption.AllDirectories);
                foreach (var nuspec in nuspecs)
                    numUpdated += UpdateNuspec(nuspec, GetCorrespondingPackagesFile(nuspec));
            }
            else
            {
                // We expect a nuspec, with an optional packages file.
                string nuspec = args[0];
                string packagesFile = args.Length == 2 ? args[1] : GetCorrespondingPackagesFile(nuspec);
                numUpdated += UpdateNuspec(nuspec, packagesFile);
            }

            sw.Stop();
            Console.WriteLine("{0}: {1} nuspec files updated in {2} msec.", ExeName, numUpdated, sw.ElapsedMilliseconds);
        }

        private static string ExeName
        {
            get
            {
                return Assembly.GetEntryAssembly().ManifestModule.Name;
            }
        }

        private static string GetCorrespondingPackagesFile(string nuspecFile)
        {
            string d = Path.GetDirectoryName(nuspecFile);
            return Path.Combine(d, "packages.config");
        }

        private static int UpdateNuspec(string nuspecFile, string packagesFile)
        {
            if (!File.Exists(nuspecFile))
            {
                Console.WriteLine("Error: nuspec file " + nuspecFile + " does not exist.");
                Environment.Exit(1);
            }

            // Allow this, you can be packaging and not have any dependencies.
            if (!File.Exists(packagesFile))
                return 0;

            var packages = GetPackages(packagesFile);
            UpdateNuspecFile(nuspecFile, packages);
            return 1;
        }

        private static List<Package> GetPackages(string packagesFile)
        {
            var result = new List<Package>();

            var packagesDoc = new XmlDocument();
            packagesDoc.Load(packagesFile);

            foreach (XmlNode pkg in packagesDoc.GetElementsByTagName("package"))
            {
                result.Add(new Package(pkg));
            }

            // Sorting by id leads to less VCS churn.
            result.Sort(delegate (Package p1, Package p2) { return p1.Id.CompareTo(p2.Id); });
            return result;
        }

        private static void UpdateNuspecFile(string nuspecFile, List<Package> packages)
        {
            if (packages == null || packages.Count == 0)
                return;

            var nuSpecDoc = new XmlDocument();
            nuSpecDoc.Load(nuspecFile);

            var dependenciesNode = nuSpecDoc.SelectSingleNode("package/metadata/dependencies");
            if (dependenciesNode == null)
            {
                dependenciesNode = nuSpecDoc.CreateElement("dependencies");
                var packageNode = nuSpecDoc.SelectSingleNode("package/metadata");
                packageNode.AppendChild(dependenciesNode);
            }

            dependenciesNode.RemoveAll();
            foreach (var pkg in packages)
            {
                var dependencyNode = nuSpecDoc.CreateElement("dependency");

                var idAttr = nuSpecDoc.CreateAttribute("id");
                idAttr.Value = pkg.Id;
                dependencyNode.Attributes.Append(idAttr);

                var verAttr = nuSpecDoc.CreateAttribute("version");
                verAttr.Value = pkg.Version;
                dependencyNode.Attributes.Append(verAttr);

                dependenciesNode.AppendChild(dependencyNode);
            }

            nuSpecDoc.Save(nuspecFile);
            Console.WriteLine("{0}: {1} updated", ExeName, nuspecFile);
        }

        private static void ShowHelpAndExit()
        {

            Console.WriteLine(@"{0} - update <dependencies> in a nuspec file.

USAGE
    {0} -r | NUSPECFILE [PACKAGESFILE]

DESCRIPTION
===========
Updates the <dependencies> node in a nuspec file with a list of dependent packages,
determined by scanning the corresponding packages.config file.

If -r is specified, the program finds all nuspec files under the current working
directory and updates them if they have a corresponding packages.config file in the
same directory. This is the recommended usage.

If NUSPECFILE is specified then only that file is updated. If PACKAGESFILE is
specified then it is used, otherwise packages.config is assumed to be in the same
directory as the NUSPECFILE.

In all usages, if the packages.config file does not exist then no action is taken
and no error is reported. An error is reported if NUSPECFILE does not exist.
",
ExeName
);

            Environment.Exit(1);
        }
    }
}
