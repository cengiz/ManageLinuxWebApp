using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace dotnetCoreManageLinux.Web
{
    public class Helper
    {
        public static string RunShellCommand(string cmd)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var escapedArgs = cmd.Replace("\"", "\\\"");

                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo("c:\\", string.Format("\"{0}\"", cmd));
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.WorkingDirectory = "c:\\";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.RedirectStandardOutput = true;

                return "";
            }

            
        }
    }
}
