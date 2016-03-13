using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace UpdateNuGetDeps
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args[0] == "--help" || args[0] == "?" || args[0] == "-h")
                ShowHelpAndExit();

            string nuSpecFile = args[0];
            if (!File.Exists(nuSpecFile))
            {
                Console.WriteLine("Error: nuspec file " + nuSpecFile + " does not exist.");
                Environment.Exit(1);
            }

            string packagesFile = args[1];
            if (!File.Exists(packagesFile))
            {
                Console.WriteLine("Error: packages.config file " + packagesFile + " does not exist.");
                Environment.Exit(1);
            }

            var packages = GetPackages(packagesFile);
            UpdateNuSpecFile(nuSpecFile, packages);
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

        private static void UpdateNuSpecFile(string nuSpecFile, List<Package> packages)
        {
            if (packages == null || packages.Count == 0)
                return;

            var nuSpecDoc = new XmlDocument();
            nuSpecDoc.Load(nuSpecFile);

            var dependenciesNode = nuSpecDoc.SelectSingleNode("package/dependencies");
            if (dependenciesNode == null)
            {
                dependenciesNode = nuSpecDoc.CreateElement("dependencies");
                var packageNode = nuSpecDoc.SelectSingleNode("package");
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

            nuSpecDoc.Save(nuSpecFile);
        }

        private static void ShowHelpAndExit()
        {
            Console.WriteLine("UpdateNuGetDeps.exe NUSPECFILE PACKAGESFILE");
            Console.WriteLine("--");
            Console.WriteLine("Updates the <dependencies> node in NUSPECFILE to include all the");
            Console.WriteLine("nuget packages you are using, as determined by scanning PACKAGESFILE.");
            Environment.Exit(1);
        }
    }
}
