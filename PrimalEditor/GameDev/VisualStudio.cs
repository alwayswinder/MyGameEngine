﻿using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

namespace PrimalEditor.GameDev
{
    enum BuildConfiguration
    {
        Debug,
        DebugEditor,
        Release,
        ReleaseEditor,
    }

    static class VisualStudio
    {
        private static ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
        private static readonly string _proID = "VisualStudio.DTE.16.0";
        private static readonly object _lock = new object();
        private static readonly string[] _buildConfigurationName = new string[] { "Debug", "DebugEditor", "Release", "ReleaseEditor" };
        private static EnvDTE80.DTE2 _vsInstance = null;
        public static bool BuildSucceeded { get; private set; } = true;
        public static bool BuildDone { get; private set; } = true;

        public static string GetConfigurationName(BuildConfiguration config) => _buildConfigurationName[(int)config];

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        private static void CallOnSTAThread(Action action)
        {
            Debug.Assert(action != null);
            var thread = new Thread(() =>
            {
                MessageFilter.Register();
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.Log(MessageType.Warning, ex.Message);
                }
                finally
                {
                    MessageFilter.Revoke();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
        private static void OpenVisualStudio_internal(string solutionPath)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker monikerTable = null;
            IBindCtx bindCtx = null;
            try
            {
                if (_vsInstance == null)
                {
                    var hResult = GetRunningObjectTable(0, out rot);
                    if (hResult < 0 || rot == null) throw new COMException($"getRunningObjectTable() returned HRESULT: {hResult:X8}");

                    rot.EnumRunning(out monikerTable);
                    monikerTable.Reset();

                    hResult = CreateBindCtx(0, out bindCtx);
                    if (hResult < 0 || bindCtx == null) throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:X8}");

                    IMoniker[] currentMoniker = new IMoniker[1];
                    while(monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
                    {
                        string name = string.Empty;
                        currentMoniker[0]?.GetDisplayName(bindCtx, null, out name);
                        if(name.Contains(_proID))
                        {
                            hResult = rot.GetObject(currentMoniker[0], out object obj);
                            if (hResult < 0 || obj == null) throw new COMException($"Running object table's GetObject returned HRESULT: {hResult:X8}");
                            EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;

                            var solutionName = string.Empty;
                            CallOnSTAThread(() =>
                            {
                                solutionName = dte.Solution.FullName;
                            });

                            if(solutionName == solutionPath)
                            {
                                _vsInstance = dte;
                                break;
                            }
                        }
                    }
                    if (_vsInstance == null)
                    {
                        Type visualStudioType = Type.GetTypeFromProgID(_proID, true);
                        _vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, "failed to open Visual Studio");
            }

            finally
            {
                if (monikerTable != null) Marshal.ReleaseComObject(monikerTable);
                if (rot != null) Marshal.ReleaseComObject(rot);
                if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);

            }
        }
        public static void OpenVisualStudio(string solutionPath)
        {
            lock (_lock) { OpenVisualStudio_internal(solutionPath); }
        }

        private static void CloseVisualStudio_internal()
        {
            CallOnSTAThread(() =>
            {
                if (_vsInstance?.Solution.IsOpen == true)
                {
                    _vsInstance.ExecuteCommand("File.SaveAll");
                    _vsInstance.Solution.Close(true);
                }
                _vsInstance?.Quit();
                _vsInstance = null;
            });
        }
        public static void CloseVisualStudio()
        {
            lock (_lock) { CloseVisualStudio_internal(); }
        }

        private static bool AddFilesToSolution_internal(string solution, string projectName, string[] files)
        {
            Debug.Assert(files?.Length > 0);
            OpenVisualStudio_internal(solution);
            try
            {
                if(_vsInstance != null)
                {
                    CallOnSTAThread(() =>
                    {
                        if (!_vsInstance.Solution.IsOpen) _vsInstance.Solution.Open(solution);
                        else _vsInstance.ExecuteCommand("File.SaveAll");

                        foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
                        {
                            if (project.UniqueName.Contains(projectName))
                            {
                                foreach (var file in files)
                                {
                                    project.ProjectItems.AddFromFile(file);
                                }
                            }
                        }
                        var cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");

                        if (!string.IsNullOrEmpty(cpp))
                        {
                            _vsInstance.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindTextView).Visible = true;
                        }
                        _vsInstance.MainWindow.Activate();
                        _vsInstance.MainWindow.Visible = true;
                    });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("failed to add files to Visual Studio project.");
                return false;
            }
            return true;
        }
        public static bool AddFilesToSolution(string solution, string projectName, string[] files)
        {
            lock (_lock) { return AddFilesToSolution_internal(solution, projectName, files); }
        }

        private static void OnBuildSolutionBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
            if (BuildDone) return; 
            Logger.Log(MessageType.Info, $"Build{project}, {projectConfig}, {platform}, {solutionConfig}");
        }
        private static void OnBuildSolutionDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (BuildDone) return;
            if (success) Logger.Log(MessageType.Info, $"Building {projectConfig} Configuration succeeded");
            else Logger.Log(MessageType.Error, $"Building {projectConfig} configuration failed");

            BuildDone = true;
            BuildSucceeded = success;
            _resetEvent.Set();
        }
        private static bool IsDebugging_internal()
        {
            bool result = false;
            CallOnSTAThread(() =>
            {
                result = _vsInstance != null &&
                    (_vsInstance.Debugger.CurrentProgram != null || _vsInstance.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgRunMode);
            });
            return result;
        }
        public static bool IsDebugging()
        {
            lock (_lock) { return IsDebugging_internal(); }
        }

        private static void BuildSolution_internal(Project project, BuildConfiguration buildConfig, bool showWindow = true)
        {
            if (IsDebugging_internal())
            {
                Logger.Log(MessageType.Error, "Visual Studio is currenty running a process.");
                return;
            }
            OpenVisualStudio_internal(project.Solution);
            BuildDone = BuildSucceeded = false;

            CallOnSTAThread(() =>
            {
                if (!_vsInstance.Solution.IsOpen) _vsInstance.Solution.Open(project.Solution);

                _vsInstance.MainWindow.Visible = showWindow;
                _vsInstance.Events.BuildEvents.OnBuildProjConfigBegin += OnBuildSolutionBegin;
                _vsInstance.Events.BuildEvents.OnBuildProjConfigDone += OnBuildSolutionDone;
            });

            var configName = GetConfigurationName(buildConfig);
            try
            {
                foreach (var pdbFile in Directory.GetFiles(Path.Combine($"{project.Path}", $@"x64\{configName}"), "*.pdb"))
                {
                    File.Delete(pdbFile);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            CallOnSTAThread(() =>
            {
                _vsInstance.Solution.SolutionBuild.SolutionConfigurations.Item(configName).Activate();
                _vsInstance.ExecuteCommand("Build.BuildSolution");
                _resetEvent.Wait();
                _resetEvent.Reset();
            });
        }
        public static void BuildSolution(Project project, BuildConfiguration buildConfig, bool showWindow = true)
        {
            lock (_lock) { BuildSolution_internal(project, buildConfig, showWindow); }
        }

        private static void Run_internal(Project project, BuildConfiguration buildConfig, bool debug)
        {
            CallOnSTAThread(() =>
            {
                if (_vsInstance != null && !IsDebugging_internal() && BuildSucceeded)
                {
                    _vsInstance.ExecuteCommand(debug ? "Debug.Start" : "Debug.StartWithoutDebugging");
                }
            });
        }
        public static void Run(Project project, BuildConfiguration buildConfig, bool debug)
        {
            lock (_lock) { Run_internal(project, buildConfig, debug); }
        }

        private static void Stop_internal()
        {
            CallOnSTAThread(() =>
            {
                if (_vsInstance != null && IsDebugging_internal())
                {
                    _vsInstance.ExecuteCommand("Debug.StopDebugging");
                }
            });
        }
        public static void Stop()
        {
            lock (_lock) { Stop_internal(); }
        }

    }
    /// <summary>
    /// //////////////////////////
    /// </summary>
    public class MessageFilter : IMessageFilter
    {
        private const int SERVERCALL_ISHANDLED = 0;
        private const int PENDINGMSG_WAITDEFPROCESS = 2;
        private const int SERVEERCALL_RETRYLATER = 2;

        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IMessageFilter newFilter, out IMessageFilter oldFilter);

        public static void Register()
        {
            IMessageFilter newFilter = new MessageFilter();
            int hr = CoRegisterMessageFilter(newFilter, out var oldFilter);
            Debug.Assert(hr >= 0, "Registering COM IMessageFilter failed.");
        }

        public static void Revoke()
        {
            int hr = CoRegisterMessageFilter(null, out var oldFilter);
            Debug.Assert(hr >= 0, "Unregistering COM IMessageFilter failed.");
        }


        int IMessageFilter.HandleInComingCall(int dwCallType, System.IntPtr hTaskCaller, int dwTickCount, System.IntPtr lpInterfaceInfo)
        {
            //returns the flag SERVERCALL_ISHANDLED. 
            return SERVERCALL_ISHANDLED;
        }


        int IMessageFilter.RetryRejectedCall(System.IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            // Thread call was refused, try again. 
            if (dwRejectType == SERVEERCALL_RETRYLATER)
            // flag = SERVERCALL_RETRYLATER. 
            {
                // retry thread call at once, if return value >=0 & 
                Debug.WriteLine("COM server busy. Retrying call to EnvDTE interface.");
                return 500;
            }
            return -1;
        }


        int IMessageFilter.MessagePending(System.IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            //return flag PENDINGMSG_WAITDEFPROCESS. 
            return PENDINGMSG_WAITDEFPROCESS;
        }
    }
    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMessageFilter
    {

        [PreserveSig]
        int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);


        [PreserveSig]
        int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);


        [PreserveSig]
        int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }
}
