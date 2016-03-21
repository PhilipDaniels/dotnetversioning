NAME
====
dotnet-setversion - create a new version number based on an existing version number and a pattern,
then write it to a file.

SYNOPSIS
========
```
dotnet-setversion.exe --verbose --avpat AVPAT --avcur AVCUR --afvpat AFVPAT --afvcur AFVCUR \
                      --aivpat AIVPAT --aivcur AIVCUR --read INFILE \
                      --write OUTFILE --what [av,afv,aiv|all]
					  
```

TYPICAL USAGE
=============

| Task                                    | Command Line                               |
|-----------------------------------------|------------------------------------------- |
|Read file and print current attribute(s) | `dnv --read foo.dll --what all`            |
|Eval pattern and print                   | `dnv --avpat "{{UtcNow}}" --what av`       |
|Update version numbers in file           | `dnv (patterns) --write foo.cs --what all` |

OPTIONS
=======
--avpat   - the pattern to be used to generate the AssemblyVersion attribute
--avcur   - the current value of the AssemblyVersion attribute
--afvpat  - the pattern to be used to generate the AssemblyFileVersion attribute
--afvcur  - the current value of the AssemblyFileVersion attribute
--aivpat  - the pattern to be used to generate the AssemblyInformationalVersion
            attribute
--aivcur  - the current value of the AssemblyInformationalVersion attribute
--read    - a file to try and read the current values and patterns from
--write   - a file to write the new version numbers to, you can specify which
            attributes should be updated using the [av,afv,aiv] list
--verbose - turn on logging to standard output

Mnemonic: "av" refers to the AssemblyVersion attribute, "afv" refers to the AssemblyFileVersion
attribute, and "aiv" refers to the AssemblyInformationalVersion attribute.

Patterns should be surrounded with quotes to escape them from the shell:

```
  dotnet-setversion --avpat "1.2.3-pre{{UtcNow}}" ...
```

DESCRIPTION
===========
dotnet-setversion applies a pattern `PAT` to a current version number `CUR` to produce a new,
updated version number. `PAT` and `CUR` may be specified on the command line or extracted from
`INFILE`, which may be of several different types. Command line parameters take priority over
those read from the `INFILE`. There are separate options to allow independent control of the three
assembly attributes - AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion.

Both the pattern and the current value may be read from `INFILE`. However, any values set on the
command line take priority.

Variables may be used in `PAT` to perform common operations such as incrementing existing numbers
and substituting dates, environment variables, or the contents of files.

dotnet-setversion can use either the traditional four-part .Net version number or the three-part
SEMVER version numbering scheme favoured by dotnet/cli. It is recommend to use SEMVER.

After the new version number(s) are determined, they are written to the `OUTFILE`. `OUTFILE`
may be, and typically will be, the same file as `INFILE`. There is no option to write to standard
output.

dotnet-setversion is designed to work with both legacy .Net applications which use Assembly
attributes and the net dotnet/cli `project.json` scheme which uses just a "version" tag. In the
former, you probably want to control each attribute independently, while for project.json it is
sufficient to simply set the "version" to an appropriate value such as "1.2.3.4-pre20160320" and
allow the dotnet tooling to set the attributes and nupkg package name automatically.

SUPPORTED FILE TYPES
====================
The supported file types are:

* A .Net dll or exe file (a compiled assembly)
* A JSON file, such as project.json
* A C# source file (ending in .cs)
* A VB.Net source file (ending in .vb)
* An F# source file (ending in .fs)

All types of file may be used as both `INFILE` and `OUTFILE`, with the exception of a compiled
assembly, which can only be used as an `INFILE`.

For a .Net source file the assembly version attributes are extracted by using regular expressions.
For a compiled assembly, the embedded attributes are extracted by reflection.

For a JSON file, the current version is determined by using a regular expression to find the first
occurence of a "version" tag. This is designed to be compatible with the new `project.json` format
used by NuGet 3 and the dotnet/cli toolset.

When writing, normally the OUTFILE will already exist. The new version will be written in a
minimal-change manner, using a regular expression to change the old version to the new. This
should preserve any particular formatting or indentation you have in your files. If the file does
not exist, then a new file will be created with the very minimum amount of information required.
In particular, a created JSON file will be a tiny snippet containing just a "version" tag.

PATTERN VARIABLES
=================
dotnet-setversion supports several variables in the pattern to allow you to perform common tasks
such as incrementing existing numbers or including dates and times. The complete list of variables
is:

* `%%Var%%` - subtitute the value of an environment variable. The variable name is not
  case-sensitive.
* `##ver.txt##` - subsitute the contents of a file. In fact, just the first line of the file is
  substituted. The filename may be absolute, such as "C:\temp\ver.txt" or relative, such as
  "ver.txt". Relative filenames are relative to the current working directory.
* `{{Inc}}` - increment a number by 1.
* `{{Inc:Reset}}` - increment a number by 1 and reset all following numbers to zero.
* `{{UtcNowDOY}}` - create a special date serial number from DateTime.Now in the format `YYDOY`
  where YY is the year number and DOY is the day-of-year, an integer in the range 1..366.
* `{{UtcNowDOY}}` - as for UtcNowDOY, but using DateTime.UtcNow instead of DateTime.Now.
* `{{Now}}` - subsitute the current value of DateTime.Now using the default format string of
  "yyMMdd-HHmmss". This pattern is designed to be accurate to the second, yet short enough that it
  is usable in a NuGet suffix string such as "pre160322-162731"; such suffix strings must be less
  than 20 characters long.
* `{{UtcNow}}` - substitute the current value of DateTime.UtcNow using the default format string
  of "yyMMdd-HHmmss".
* `{{Now:fmt}}` - subsitute the current value of DateTime.Now using `fmt` as the format string
  (all standard .Net format specifiers are possible).
* `{{UtcNow:fmt}}` - subsitute the current value of DateTime.UtcNow using `fmt` as the format
  string (all standard .Net format specifiers are possible).
* `{{Same}}` - a no-op, the value in that part of the current version number is not changed.
* `{{GitBranch}} - the current Git branch.
* `{{GitCommit}} - the current Git commit sha (the entire sha).
* `{{GitCommit:N}} - the first N characters of the current Git commit sha. N is an integer.
* `{{MachineName}}` - the value of Environment.MachineName.
* `{{OSVersion}}` - the value of Environment.OSVersion.
* `{{UserName}}` - the value of Environment.UserName.
* `{{UserDomainName}}` - the value of Environment.UserDomainName.

If the built-in variables are not sufficient for your needs, the ability to subsitute the value of
an environment variable or a file provides a "get out of jail free" card which you can use to
accomplish any versioning scheme - write a batch file or PowerShell script to generate a file with
the desired data then use dotnet-setversion to write it into the assembly attributes.

VARIABLE EXAMPLES
=================
Current     Pattern                           New         Notes
-------     -------                           ---         -----
1.0.0.0     1.2.3.4                           1.2.3.4     no variables, pattern is taken literally
1.10.0.1    1.{{Inc}}.0.{{Inc}}               1.11.0.2    multiple Incs are ok.
1.10.0      1.0.%%Ver%%                       1.0.32      assuming the environment variable has the value "32"
1.10.0      %%Ver%%                           2.0.7       assuming the environment variable has the value "2.0.7"
1.10.0      ##c:\tenp\ver.txt##               2.0.7       assuming the file has the value "2.0.7" on its first line
1.10.0      %%Major%%.##Minor.txt##.{{Inc}}   3.17.1      combos are acceptable
1.10.0      1.10.0-pre{{UtcNow}}              1.10.0-pre160320-173022     good for monotonic build numbers
1.12.0      1.{{Same}}.7                      1.12.7
1.0.0       1.{{UtcNowDOY}}.{{UtcNow:HHmm}}   1.15209.1632          one approach to encoding the date and time

Note that the minor and revision numbers may not exceed 2^16 - 1, or 65535. This precludes using a
format such as YYYYMMDD, and even YYMMDD won't work, but the date DOY serial numbers will fit, and
the nth day of the year is trivial to discover using Google.

THE ASSEMBLY ATTRIBUTES
=======================
* AssemblyVersion - this is the most important attribute, as it is used by the CLR during the
assembly load process. However, it is not visible in Windows' property pages for the assembly.

* AssemblyFileVersion - intended to identify an individual build of the assembly. Ignored by the
CLR. It is displayed by Windows as "File version" on an assembly's property details page.

* AssemblyInformationalVersion - an arbitrary string. Ignored by the CLR. It is displayed by
Windows as the "Product version" on an assembly's property details. However, it is often too long
to be completely seen, so a disassembly tool such as ILSpy or dotPeek should be used to view it
(or dotnet-getversion can be used to dump it to the standard output).

Modern practice is to make AssemblyVersion and AssemblyFileVersion identical, and to use SEMVER
number versioning for them both. Use the AssemblyInformationalVersion attribute to store an
arbitrary string which will provide traceability back to your build process, for example "Build
Configuration = Release, commit 12af548g on branch master at time 2016-03-12 15:34 UTC." 

GUIDELINES
==========
* Developers should manually control the MAJOR.MINOR.REVISION components according to SEMVER
  standards. Do not set them based on CI build numbers because this will make it difficult to
  inter-operate with local builds (see "Eliminating the NuGet Shuffle" below).

* For legacy .Net projects, use AssemblyInformationalVersion to provide traceability back to
  the exact set of code used to build the project. For project.json projects, you achieve the
  same thing simply by using a longer version string such as
  "1.2.3-pre20160320173022-commit-12aa63-master".

* Pre-release builds should use a "-pre{{UtcNow}}" or similar suffix.

* Don't use local dates, always use UTC.

* A pre-release build is anything not built on the master branch.

* For legacy .Net projects, create a separate AssemblyInfo.gen.cs file with the
  following contents
  
  [assembly: AssemblyVersion("1.0.0")]
  [assembly: AssemblyFileVersion("1.0.0")]
  [assembly: AssemblyInformationalVersion("1.0.0")]

* For both legacy and project.json builds, use the MSBuild PreBuild event to run a script which
  sets the version number. This event still works in new dotnet/cli projects, but you will need
  to edit the .xproj file manually. Pass the build configuration to the script, as well as any
  other information you may need. Example:

  <Target Name="BeforeBuild">
    <Exec Command="set_version.cmd $(Configuration)" WorkingDirectory="$(SolutionDir)\scripts" />
  </Target>

  The home repository for dotnet-setversion contains a complete worked example.
  
* If building several projects within one solution, consider creating a single AssemblyInfo.gen.cs
  file and including it in all your projects via Visual Studio's "Add as link" function.

* Each developer should create a LocalNuGetFeed on their machine and set it up as a package
  source, but don't automatically push all local builds to your LocalNuGetFeed. Write a batch
  file to do it on demand ("push last build"). Having your own local feed allows you to test
  packages in their downstream users without having to push to your CI server (see "Eliminating
  the NuGet Shuffle" below).

ELIMINATING THE NUGET SHUFFLE
=============================
Determinging version numbers on both CI and local machine in a way that is compatible.

Building packages. Building a project against multiple frameworks.

Pushing packages to a feed.

Referencing packages in projects - using NuGet packages within one solution.




SEE ALSO
========
dotnet-getversion, which can read a current version number and write it to standard output.

Project.json - https://github.com/aspnet/Home/wiki/Project.json-file
Semantic versioning (SEMVER) - http://semver.org/
Assembly attributes: http://stackoverflow.com/questions/64602/
