What is ManifestUtil?
---------------------
ManifestUtil is a utility that will create .pkg files for driver DLLs.

How to Use ManifestUtil?
------------------------
ManifestUtil will scan a directory for .dll files containing an embedded JSON
resource. It will attempt to create a .pkg file for each of these.

The directory can be specified as a command-line argument, or if left
unspecified, it defaults to the present directory. This means it can be run
one of three ways:

    1. From the directory containing the DLLs, run ManifestUtil.exe
    2. From any directory, run ManifestUtil.exe "C:\Path\To\Directory"
    3. Copy ManifestUtil.exe to the directory containing the DLLs and
       double-click it

How to Set Up ManifestUtil to Automatically Run in Visual Studio
----------------------------------------------------------------
These EnvironmentEvents.vb and ManifestUtil.vb Visual Studio macros allow you
to create pkgs using ManifestUtil at the same time a dll is compiled without
having to manually call ManifestUtil.exe yourself

1.  Open ManifestUtil.vb in a VSCode/Notepad and change the location of "dim
    ManifestUtilPath as String" with the location of ManifestUtil.exe on your
    computer. (default path is packages\ManifestUtil\lib)
2.  Open the Macros IDE in Visual Studio (Tools -> Macros -> Macros IDE -> MyMacros) or (Alt+F11)
3.  Copy the two subs from the bottom of EnvironmentEvents.vb and add them to your EnvironmentEvents module, just before 'End Module'
4.  Right-click MyMacros, then Add -> Add Existing Item, and add ManifestUtil.vb
5.  Save MyMacros
6.  Right-click > Build MyMacros
7.  Close Macros IDE and close Visual Studio. Select save if a popup appears

Now, when you build a single driver project, and PKG should appear in the
build output folder, next to the driver DLL next time you open Visual Studio
