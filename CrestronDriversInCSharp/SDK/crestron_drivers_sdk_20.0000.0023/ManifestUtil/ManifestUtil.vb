Imports System
Imports System.IO
Imports EnvDTE
Imports EnvDTE80
Imports EnvDTE90

Public Module ManifestUtil

    Public Last_OnBuildBegin_Scope As EnvDTE.vsBuildScope
    Public Last_OnBuildBegin_Action As EnvDTE.vsBuildAction

    'modify this path with the location of ManifestUtil.exe on your computer
    Dim ManifestUtilPath As String = "packages\ManifestUtil\lib\ManifestUtil.exe"

    Public Sub MakePkg(ByVal Project As String)
        Try
            If Last_OnBuildBegin_Action <> vsBuildAction.vsBuildActionBuild Then Exit Sub
            If Last_OnBuildBegin_Scope <> vsBuildScope.vsBuildScopeProject Then Exit Sub
            If Path.GetFileNameWithoutExtension(Project) = "RunnableStartupProject" Then Exit Sub

            Dim dll = Path.GetFileNameWithoutExtension(Project) + ".dll"
            Dim sln = Path.GetDirectoryName(DTE.Solution.FullName)
            Dim prj = Path.Combine(sln, Path.GetDirectoryName(Project))
            Dim src = ManifestUtilPath

            'for internal CCD/RAD Crestron devs using paket to get dependencies
            If (ManifestUtilPath.StartsWith("packages")) Then
                src = Path.Combine(sln, ManifestUtilPath)
            End If

            If (File.Exists(src)) Then
                For Each dll In Directory.GetFiles(prj, dll, SearchOption.AllDirectories)
                    If dll.Contains("\obj\") Then Continue For
                    Dim dst = Path.Combine(Path.GetDirectoryName(dll), "ManifestUtil.exe")
                    If Not File.Exists(dst) Then File.Copy(src, dst)
                    Dim proc = System.Diagnostics.Process.Start(Chr(34) + dst + Chr(34), Chr(34) + Path.GetDirectoryName(dll) + Chr(34))
                    proc.WaitForExit()
                    File.Delete(dst)
                    Dim pkg = dll.Replace(".dll", ".pkg")
                    If File.Exists(pkg) Then
                        System.Diagnostics.Process.Start(Chr(34) + Path.GetDirectoryName(dll) + Chr(34))
                    End If
                Next
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.Print(ex.Message + ex.StackTrace)
        End Try
    End Sub

End Module
