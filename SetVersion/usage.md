NAME
====
dnv - create a new version number based on an existing version number and a pattern, then write it
to a file.

SYNOPSIS
========
```
dnv --verbose --avpat AVPAT --avcur AVCUR --afvpat AFVPAT --afvcur AFVCUR \
    --aivpat AIVPAT --aivcur AIVCUR --read INFILE --write OUTFILE --what [av,afv,aiv|all]			  
```

COMMON USE CASES
================

| Task                                     | Command Line                               |
|------------------------------------------|------------------------------------------- |
| Update version numbers in file           | `dnv (patterns) --write foo.cs --what all` |
| Read file and print current attribute(s) | `dnv --read foo.dll --what all`            |
| Evaluate pattern and print to stdout     | `dnv --avpat "{{UtcNow}}" --what av`       |

OPTIONS
=======
`--avpat` the pattern to be used to generate the AssemblyVersion attribute

`--avcur` the current value of the AssemblyVersion attribute

`--afvpat` the pattern to be used to generate the AssemblyFileVersion attribute

`--afvcur` the current value of the AssemblyFileVersion attribute

`--aivpat` the pattern to be used to generate the AssemblyInformationalVersion attribute

`--aivcur` the current value of the AssemblyInformationalVersion attribute

`--read` a file to try and read the current values and patterns from

`--write` a file to write the new version numbers to, you can specify which attributes should be
          updated using the [av,afv,aiv] list

`--verbose` turn on logging to standard output

Mnemonic: "av" refers to the `AssemblyVersion` attribute, "afv" refers to the `AssemblyFileVersion`
attribute, and "aiv" refers to the `AssemblyInformationalVersion` attribute.

Patterns should be surrounded with quotes to escape them from the shell:

```
  dnv --avpat "1.2.3-pre{{UtcNow}}" ...
```

DESCRIPTION
===========
dnv applies a pattern `PAT` to a current version number `CUR` to produce a new, updated version
number. `PAT` and `CUR` may be specified on the command line or extracted from `INFILE`, which may
be of several different types. Command line parameters take priority over those read from the
`INFILE`. There are separate options to allow independent control of the three assembly attributes
- AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion.

Variables may be used in `PAT` to perform common operations such as incrementing existing numbers
and substituting dates, environment variables, or the contents of files. `CUR` is only used when
the {{Inc}} pattern is being used to add 1 to an existing number, for other patterns there is no
need to specify `CUR`.

dnv can use either the traditional four-part .Net version number or the three-part SEMVER version
numbering scheme favoured by .Net Core. SEMVER is recommended.

After the new version number(s) are determined, they are written to the `OUTFILE`. `OUTFILE` may
be, and typically will be, the same file as `INFILE`. If no `OUTFILE` is specified then the new
version numbers are written to the standard output, this allows dnv to be used as a simple pattern
evaluator.

dnv is designed to work with both legacy .Net applications which use Assembly attributes and the
net .Net Core `project.json` scheme which uses just a "version" tag. In the former, you probably
want to control each attribute independently, while for .Net Core projects you will probably also
want to set the AssemblyInformationalVersion attribute in a source file, since it does not appear
to be possible to set this attribute via project.json. If you set "version" to a value such as
"1.2.3-pre20160320" then the new tooling will build a nupkg with an appropriate name.

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
used by NuGet 3 and the .Net Core CLI toolset.

When writing, normally the `OUTFILE` will already exist. The new version will be written in a
minimal-change manner, using a regular expression to change the old version to the new. This should
preserve any particular formatting or indentation you have in your files. If the file does not
exist, then a new file will be created with the very minimum amount of information required. In
particular, a created JSON file will be a tiny snippet containing just a "version" tag.

PATTERN VARIABLES
=================
dnv supports several variables in the pattern to allow you to perform common tasks such as
incrementing existing numbers or including dates and times. The complete list of variables is:

* `%%Var%%` subtitute the value of an environment variable. The variable name is not case-
   sensitive.
* `##ver.txt##` substitute the contents of a file. In fact, just the first line of the file is
  substituted. The filename may be absolute, such as "C:\temp\ver.txt" or relative, such as
  "ver.txt", which will be resolved with respect to the current working directory.
* `{{Inc}}` increment a number by 1.
* `{{Inc:Reset}}` increment a number by 1 and reset all following numbers to zero.
* `{{NowDOY}}` create a special date serial number from DateTime.Now in the format `YYDOY` where YY
   is the year number and DOY is the day-of-year, an integer in the range 1..366.
* `{{UtcNowDOY}}` as for NowDOY, but using DateTime.UtcNow instead of DateTime.Now.
* `{{Now}}` subsitute the current value of DateTime.Now using the default format string of
  "yyMMdd-HHmmss". This pattern is designed to be accurate to the second, yet short enough that it
  is usable in a NuGet suffix string such as "pre160322-162731"; such suffix strings must be less
  than 20 characters long.
* `{{UtcNow}}` substitute the current value of DateTime.UtcNow using the default format string
  of "yyMMdd-HHmmss".
* `{{Now:fmt}}` subsitute the current value of DateTime.Now using `fmt` as the format string
  (all standard .Net format specifiers are possible).
* `{{UtcNow:fmt}}` subsitute the current value of DateTime.UtcNow using `fmt` as the format string
  (all standard .Net format specifiers are possible).
* `{{Same}}` a no-op, the value in that part of the current version number is not changed.
* `{{GitBranch}}` the current Git branch.
* `{{GitCommit}}` the current Git commit sha (the entire sha).
* `{{GitCommit:N}}` the first N characters of the current Git commit sha. N is an integer.
* `{{MachineName}}` the value of Environment.MachineName.
* `{{OSVersion}}` the value of Environment.OSVersion.
* `{{UserName}}` the value of Environment.UserName.
* `{{UserDomainName}}` the value of Environment.UserDomainName.

If the built-in variables are not sufficient for your needs, the ability to subsitute the value of
an environment variable or a file provides a "get out of jail free" card which you can use to
accomplish any versioning scheme - write a batch file or PowerShell script to generate a file with
the desired data then use dnv to write it into the assembly attributes.

VARIABLE EXAMPLES
=================

| Current  | Pattern                          | New Version             | Notes                                                     |
| -------- | -------------------------------- | ----------------------- | --------------------------------------------------------- |
| 1.0.0.0  | 1.2.3.4                          | 1.2.3.4                 | no variables, pattern is taken literally                  |
| 1.10.0.1 | 1.{{Inc}}.0.{{Inc}}              | 1.11.0.2                | multiple Incs are ok.                                     |
| 1.10.0   | 1.0.%%Ver%%                      | 1.0.32                  | assuming the environment variable has the value "32"      |
| 1.10.0   | %%Ver%%                          | 2.0.7                   | assuming the environment variable has the value "2.0.7"   |
| 1.10.0   | ##c:\temp\ver.txt##              | 5.6.7                   | assuming the file has the value "5.6.7" on its first line |
| 1.10.0   | %%Major%%.##Minor.txt##.{{Inc}}  | 3.17.1                  | combos are acceptable                                     |
| 1.10.0   | 1.10.0-pre{{UtcNow}}             | 1.10.0-pre160320-173022 | good for monotonic build numbers                          |
| 1.12.0   | 1.{{Same}}.7                     | 1.12.7                  | no-op example                                             |
| 1.0.0    | 1.{{UtcNowDOY}}.{{UtcNow:HHmm}}  | 1.16209.1632            | one approach to encoding the date and time                |

Note that the minor and revision numbers may not exceed 2^16 - 1, or 65535. This precludes using a
format such as `YYYYMMDD`, and even `YYMMDD` won't work, but the date `DOY` serial numbers will
fit, and the nth day of the year is trivial to discover using Google.

THE ASSEMBLY ATTRIBUTES
=======================
* **AssemblyVersion** this is the most important attribute, as it is used by the CLR during the
assembly load process. However, it is not visible in Windows' property pages for the assembly.

* **AssemblyFileVersion** intended to identify an individual build of the assembly. Ignored by the
CLR. It is displayed by Windows as "File version" on an assembly's property details page.

* **AssemblyInformationalVersion** an arbitrary string. Ignored by the CLR. It is displayed by
Windows as the "Product version" on an assembly's property details. However, it is often too long
to be completely seen, so a disassembly tool such as ILSpy or dotPeek should be used to view it
(or dnv can be used to dump it to the standard output).

Modern practice is to make AssemblyVersion and AssemblyFileVersion identical, and to use SEMVER
number versioning for them both. Use the AssemblyInformationalVersion attribute to store an
arbitrary string which will provide traceability back to your build process, for example "Build
Configuration = Release, commit 12af548g on branch master at time 2016-03-12 15:34 UTC." 

GUIDELINES
==========
* Developers should manually control the MAJOR.MINOR.REVISION components according to SEMVER
  standards. Do not set them based on CI build numbers because this will make it difficult to
  inter-operate with local builds (see "Eliminating the NuGet Shuffle" below).

* Use AssemblyInformationalVersion to provide traceability back to the exact set of code used
  to build the project.

* Pre-release builds should use a "-pre{{UtcNow}}" or similar suffix to generate monotonically
  increasing build numbers.

* Don't use local dates, always use UTC.

* A pre-release build is anything not built on the master branch.

* For legacy .Net projects, create a separate AssemblyInfo.ver.cs file with the
  following contents:

  ```
  [assembly: AssemblyVersion("1.0.0")]
  [assembly: AssemblyFileVersion("1.0.0")]
  [assembly: AssemblyInformationalVersion("1.0.0")]
  ```

* For project.json projects, you will also need to create AssemblyInfo.ver.cs but it should
  only contain the AssemblyInformationalVersion attribute as the other two are controlled
  via the "version" tag in project.json:

  ```
  [assembly: AssemblyInformationalVersion("1.0.0")]
   ```

* Don't check in AssemblyInfo.ver.cs, exclude it from your VCS.

* If building several projects within one solution, consider creating a single AssemblyInfo.ver.cs
  file and including it in all your projects via Visual Studio's "Add as link" function.

* Don't set version numbers or pack on every build because it slows things down. Instead,
  create some PowerShell scripts to do this and run them manually from the Package Manager
  Console as and when required (see Examples in the repo for this project).

* Each developer should create a directory on their machine to act as a drop directory
  for nupkg files and as a NuGet feed. This allows developers to build packages locally
  in the same way that it happens on a CI server, and is key to eliminating the "NuGet Shuffle".

ELIMINATING THE NUGET SHUFFLE
=============================
The "NuGet Shuffle" occurs when you have a chain of packages dependent upon each other

```
   D -> C -> B -> A
```

Here D depends on package C, which depends on package B, which in turn depends on package A.
If a developer working on D needs to make a change in package A and the only way to build
all the packages is using a CI server then it can be a time consuming and tedious process
to make such changes. To avoid this

* Developers need to be able to build packages locally - Solution: use a pack script
  like those in the examples and drop the nupkg files into a local, well known directory
  such as LOCAL_NUGET_DIR.

* Developers need to be able to install packages from their local feed - Solution: setup
  the LOCAL_NUGET_DIR as a NuGet feed.

* Package version numbers need to be monotonic, so that packages that the developer
  builds locally will supercede those available from offical NuGet feeds. Solution: use
  a monotonically increasing version number such as "1.2.3-pre{{UtcNow}}".

* As a further aid to developer productivity, incremementing version numbers and creating
  NuGet packages should only happen on demand, it does not need to happen on every build.
  Solution: use PowerShell scripts, such scripts can easily be run from the Package
  Manager Console window.


SEE ALSO
========
* Project.json - https://github.com/aspnet/Home/wiki/Project.json-file
* Semantic versioning (SEMVER) - http://semver.org/
* Assembly attributes: http://stackoverflow.com/questions/64602/
* Project Home on Github: https://github.com/PhilipDaniels/dotnetversioning
* EchoArgs (handy for debugging PowerShell scripts): http://ss64.com/ps/call.html
* rxrepl: a Windows command line tool for performing regex replacements in files - 
  https://sites.google.com/site/regexreplace/
* fnr: another Windows command line regex tool - https://findandreplace.codeplex.com/
