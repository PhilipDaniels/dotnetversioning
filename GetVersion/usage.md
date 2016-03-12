NAME
====
dotnet-getversion - print version attributes from a file or assembly.

SYNOPSIS
========
```
dotnet-getversion.exe [--av | --afv | --aiv] FILENAME
```

OPTIONS
=======
--av   - print the `AssemblyVersion` attribute
--afv  - print the `AssemblyFileVersion` attribute
--aiv  - print the `AssemblyInformationalVersion` attribute

If no options are specified then `--av` is assumed.

DESCRIPTION
===========
Write the current version of `FILENAME` to the standard output, where
`FILENAME` may be:

* A .Net dll or exe file (a compiled assembly)
* A C# source file (ending in .cs)
* A VB.Net source file (ending in .vb)
* An F# source file (ending in .fs)
* A JSON file, such as project.json

For a .Net source file or compiled assembly, the assembly version attributes are found and printed.

For a JSON file, the current version is determined by using a regular expression to find the first
occurence of a "version" tag. The syntax accepted by the JSON file reader is the `project.json`
format used by NuGet 3 and the new dotnet/cli project, and it only includes a "version" tag.

To simplify the parsing of results printed by this program, only one version attribute may be
printed at a time.

SEE ALSO
========
Project.json - https://github.com/aspnet/Home/wiki/Project.json-file

For the more general case of search and replace in text files using arbitrary regular expressions,
use PowerShell, Cygwin, or the fnr.exe or rxrepl.exe tools mentioned here in this
Stack Overflow article: http://superuser.com/questions/339118/regex-replace-from-command-line
