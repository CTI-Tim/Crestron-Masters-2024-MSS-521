Option Strict Off
Option Explicit Off
Imports System
Imports EnvDTE
Imports EnvDTE80
Imports EnvDTE90
Imports System.Diagnostics

Public Module EnvironmentEvents

    '#Region "Automatically generated code, do not modify"
    'Automatically generated code, do not modify
    'Event Sources Begin
    <System.ContextStaticAttribute()> Public WithEvents DTEEvents As EnvDTE.DTEEvents
    <System.ContextStaticAttribute()> Public WithEvents DocumentEvents As EnvDTE.DocumentEvents
    <System.ContextStaticAttribute()> Public WithEvents WindowEvents As EnvDTE.WindowEvents
    <System.ContextStaticAttribute()> Public WithEvents TaskListEvents As EnvDTE.TaskListEvents
    <System.ContextStaticAttribute()> Public WithEvents FindEvents As EnvDTE.FindEvents
    <System.ContextStaticAttribute()> Public WithEvents OutputWindowEvents As EnvDTE.OutputWindowEvents
    <System.ContextStaticAttribute()> Public WithEvents SelectionEvents As EnvDTE.SelectionEvents
    <System.ContextStaticAttribute()> Public WithEvents BuildEvents As EnvDTE.BuildEvents
    <System.ContextStaticAttribute()> Public WithEvents SolutionEvents As EnvDTE.SolutionEvents
    <System.ContextStaticAttribute()> Public WithEvents SolutionItemsEvents As EnvDTE.ProjectItemsEvents
    <System.ContextStaticAttribute()> Public WithEvents MiscFilesEvents As EnvDTE.ProjectItemsEvents
    <System.ContextStaticAttribute()> Public WithEvents DebuggerEvents As EnvDTE.DebuggerEvents
    <System.ContextStaticAttribute()> Public WithEvents ProjectsEvents As EnvDTE.ProjectsEvents
    <System.ContextStaticAttribute()> Public WithEvents TextDocumentKeyPressEvents As EnvDTE80.TextDocumentKeyPressEvents
    <System.ContextStaticAttribute()> Public WithEvents CodeModelEvents As EnvDTE80.CodeModelEvents
    <System.ContextStaticAttribute()> Public WithEvents DebuggerProcessEvents As EnvDTE80.DebuggerProcessEvents
    <System.ContextStaticAttribute()> Public WithEvents DebuggerExpressionEvaluationEvents As EnvDTE80.DebuggerExpressionEvaluationEvents

    'Event Sources End
    'End of automatically generated code
    '#End Region    

    Private Sub BuildEvents_OnBuildBegin(ByVal Scope As EnvDTE.vsBuildScope, ByVal Action As EnvDTE.vsBuildAction) Handles BuildEvents.OnBuildBegin
        ManifestUtil.Last_OnBuildBegin_Scope = Scope
        ManifestUtil.Last_OnBuildBegin_Action = Action
    End Sub

    Private Sub BuildEvents_OnBuildProjConfigDone(ByVal Project As String, ByVal ProjectConfig As String, ByVal Platform As String, ByVal SolutionConfig As String, ByVal Success As Boolean) Handles BuildEvents.OnBuildProjConfigDone
        If Success Then ManifestUtil.MakePkg(Project)
    End Sub

End Module

