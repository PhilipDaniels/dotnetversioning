# dnv
Experimental tools for versioning and packing .Net projects - both traditional ones and the new
.Net Core "project.json" style projects.

Frustration with the CI (Bamboo) based build system we have to use at work started this experiment
in versioning and packing projects; it was also inspired by the fact that the new .Net Core
system can build a package directly for you by turning on a checkbox (though that has turned out
to be irrelevant, because you don't want to pack on every build, for speed reasons).

One of my aims is to eliminate the "NuGet Shuffle" - this occurs when you have project D which
depends on package C which depends on package B which depends on package A. If you have a feature
in D which needs you to change something in A, it can take a long time if you have to push
everything up to your CI server, wait for it to build, update packages etc. By coming up with a
numbering scheme for pre-release versions and following a convention of having a local drop dir for
nupkg files which also serves as a NuGet feed in Visual Studio, I believe this can be sped up
significantly.

This repo has two exes: `dnv.exe` from the SetVersion project and `updeps.exe` from the
UpdateNuGetDeps project, both in the DotNetVersioning.sln. It is recommended that you drop them
into a folder on your path.

`updeps` just updates the <dependencies> node in a nuspec file, which you should do prior to
packing. It can be run recursively on all nuspecs under a folder just by doing

```
  updeps -r
```

More interesting is `dnv.exe`, which can apply patterns such as {{UtcNow}} or {{GitCommit}} to
generate version numbers and them write the result into the assembly version attributes in both new
(project.json) and old (csproj) style projects. It supports C#, VB.Net and F# source syntax. dnv
can also read the current version number(s) from an assembly and print them out.

See dnv's [usage.md](https://github.com/PhilipDaniels/dotnetversioning/blob/master/SetVersion/usage.md)
which is embedded into the exe and can be called up using "--help" as an argument.

There are also some example projects, which have two scripts, `setver.ps1` to set the version
number, and `pack_and_push.ps1` to do the packing. Typically you would run these scripts manually
when required from the Package Manager Console, or from a build step on a CI system. The scripts
work best if you have an environment variable called LOCAL_NUGET_DIR which points to a folder into
which to drop the nupkg files.

You can download in the [Zip File](dnv.zip) a copy of dnv and updeps compiled for .Net framework v2
(which should work on 2 and everything after that).

# Getting started
Download the exes and put them in a folder on your path (I usually create a bin folder in my home
directory to hold these types of programs). Take a look through the examples, and copy the
setver.ps1 and pack_and_push.ps1 scripts into your solution and adjust as required.


# Examples
### Example1
Build independent packages in traditional .Net project.

### Example2
Same idea, but in new project.json style.

### Example3
Build a set of related projects in one solution with one AssemblyInfo.ver.cs and link the projects
by NuGet.


# TODO
1 .targets file and MSBuild/NuGet integration?

2. How does dnx actually use the "version" in project.json? They default to "1.0.0-*"
   what is the '*' for? If we set project.json's version to "1.2.3-pre"
   we get 1.2.3-pre.nupkg and 1.2.3-pre.symbols.nupkg and:

```
    [assembly: AssemblyFileVersion("1.2.3.0")]
    [assembly: AssemblyInformationalVersion("1.2.3-pre")]
    [assembly: AssemblyVersion("1.2.3.0")]
```

  If we put "version": "1.2.3.4-pre99", we get
  ClassLibrary1.1.2.3.4-pre99.nupkg (plus symbols) with

```
    [assembly: AssemblyFileVersion("1.2.3.4")]
    [assembly: AssemblyInformationalVersion("1.2.3.4-pre99")] << can be as long as required.
    [assembly: AssemblyVersion("1.2.3.4")]
```
