function DelFile
{
	param ([string]$file)

	if (Test-Path $file)
	{
		Remove-Item -Force $file
	}
}

DelFile dnv.zip
DelFile dnv.exe
DelFile updeps.exe

cp SetVersion\bin\Release\dnv.exe dnv.exe
cp UpdateNuGetDeps\bin\Release\updeps.exe updeps.exe
7za a dnv.zip dnv.exe updeps.exe

DelFile dnv.exe
DelFile updeps.exe

