﻿using System;
/// OSVR-Config
///
/// <copyright>
/// Copyright 2016 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>
/// 
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace OSVRConfig
{
    class Program
    {
        static Process StartFrontendProcess()
        {
            return Process.Start("http://localhost:5000");
        }

        static Process StartBackendProcess()
        {
            var appRoot = Path.Combine(Environment.CurrentDirectory, "approot");
            var runtimes = Path.Combine(appRoot, "runtimes");
            foreach(var runtime in Directory.GetDirectories(runtimes))
            {
                var webCmd = Path.Combine(runtime, "bin", "dnx.exe");
                var arguments = "--project \"packages/ConfigUtil/1.0.0/root\" --configuration Release web";
                var startInfo = new ProcessStartInfo(webCmd);
                startInfo.WorkingDirectory = appRoot;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                return Process.Start(startInfo);
            }
            return null;
        }

        static void Main(string[] args)
        {
            var backendProcess = StartBackendProcess();
            if(backendProcess == null)
            {
                Console.WriteLine("Could not find the dnx runtime. This may happen if the OSVR-Config backend did not build successfully.");
                return;
            }
            backendProcess.BeginOutputReadLine();
            backendProcess.OutputDataReceived += (sender, e) =>
            {
                // The backend outputs this to standard output
                // to signal for us to kill it. We can remove this
                // once Environment.Exit makes it into the release version
                // of CoreCLR.
                if (e.Data != null)
                {
                    if (e.Data.Contains("OSVR_Config_backend_kill_signal"))
                    {
                        backendProcess.Kill();
                    }
                    else
                    {
                        // redirect to launcher's output
                        Console.WriteLine(e.Data);
                    }
                }
            };
            Thread.Sleep(TimeSpan.FromSeconds(2.0));
            var frontendProcess = StartFrontendProcess();
            backendProcess.WaitForExit();
        }
    }
}