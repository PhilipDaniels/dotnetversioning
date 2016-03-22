# Copy resultant exes to the bin folder in my home dir, which is on my path.

Copy-Item SetVersion\bin\Release\dnv.exe $ENV:USERPROFILE\bin -Verbose
Copy-Item UpdateNuGetDeps\bin\Release\updeps.exe $ENV:USERPROFILE\bin -Verbose
