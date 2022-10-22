/********************************************************************************************
        Author: Sergiy Stoyan
        systoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cliver.Win
{
    public static partial class ProcessRoutines
    {
        /// <summary>
        /// Makes processes live no longer than this object
        /// </summary>
        public class AntiZombieGuard
        {
            void initialize()
            {
                // This feature requires Windows 8 or later. To support Windows 7 requires
                //  registry settings to be added if you are using Visual Studio plus an
                //  app.manifest change.
                //  http://qaru.site/questions/8653/how-to-stop-the-visual-studio-debugger-starting-my-process-in-a-job-object/60668#60668
                //  http://qaru.site/questions/8015/kill-child-process-when-parent-process-is-killed/57475#57475
                //if (Environment.OSVersion.Version < new Version(6, 2))
                //    return;

                //string jobName = "AntiZombieJob_" + Process.GetCurrentProcess().Id;//Can be NULL. If it's not null, it has to be unique.
                jobHandle = WinApi.Kernel32.CreateJobObject(IntPtr.Zero, null);
                if (jobHandle == null)
                    throw new Exception("CreateJobObject: " + ErrorRoutines.GetLastErrorMessage());
                WinApi.Kernel32.JOBOBJECT_BASIC_LIMIT_INFORMATION jbli = new WinApi.Kernel32.JOBOBJECT_BASIC_LIMIT_INFORMATION();
                jbli.LimitFlags = WinApi.Kernel32.JOBOBJECTLIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
                WinApi.Kernel32.JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new WinApi.Kernel32.JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
                extendedInfo.BasicLimitInformation = jbli;
                int length = Marshal.SizeOf(typeof(WinApi.Kernel32.JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
                try
                {
                    Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);
                    if (!WinApi.Kernel32.SetInformationJobObject(jobHandle, WinApi.Kernel32.JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
                        throw new Exception("SetInformationJobObject: " + ErrorRoutines.GetLastErrorMessage());
                }
                finally
                {
                    Marshal.FreeHGlobal(extendedInfoPtr);
                    extendedInfoPtr = IntPtr.Zero;
                }
            }
            // Windows will automatically close any open job handles when our process terminates.
            // When the job handle is closed, the child processes will be killed.
            IntPtr jobHandle = IntPtr.Zero;
           readonly HashSet<IntPtr> trackedProcessHandles = new HashSet<IntPtr>();

            ~AntiZombieGuard()
            {
                KillTrackedProcesses();
            }

            public bool IsProcessTracked(Process process)
            {
                return trackedProcessHandles.Contains(process.Handle); 
            }

            public void KillTrackedProcesses()
            {
                lock (This)
                {
                    if (jobHandle == IntPtr.Zero)
                        return;
                    WinApi.Kernel32.CloseHandle(jobHandle);
                    jobHandle = IntPtr.Zero;
                    trackedProcessHandles.Clear();
                }
            }

            //!!!After a process is associated with a job, the association cannot be broken. 
            //public void Untrack(Process process)
            //{
            //}

            public void Track(Process process)
            {
                lock (This)
                {
                    //All processes associated with a job must run in the same session. 
                    //A job is associated with the session of the first process to be assigned to the job.
                    if (jobHandle == IntPtr.Zero)
                        initialize();
                    if (trackedProcessHandles.Contains(process.Handle))
                        return;
                    if (!WinApi.Kernel32.AssignProcessToJobObject(jobHandle, process.Handle))
                        throw new Exception("!AssignProcessToJobObject()", ErrorRoutines.GetLastError());
                    trackedProcessHandles.Add(process.Handle);
                }
            }

            public readonly static AntiZombieGuard This = new AntiZombieGuard();
        }
    }
}