using System.Diagnostics;
using System.Text;

namespace GodotPatcher.Tools;

public static class ConsoleHelper
{
    public static event EventHandler<string> OutputReceived;

    public static string ExecuteCommand(string command)
    {
        // Determine the operating system
        bool isWindows = System.Runtime.InteropServices.RuntimeInformation
            .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

        // Create a new process to run the command
        Process process = new Process();

        // Configure the process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (isWindows)
        {
            // For Windows, use PowerShell
            startInfo.FileName = "powershell";
            startInfo.Arguments = $"-Command \"{command}\"";
        }
        else
        {
            // For Linux, use bash
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = $"-c \"{command}\"";
        }

        process.StartInfo = startInfo;

        // StringBuilder to store the output
        StringBuilder output = new StringBuilder();
        string currentLine = "";

        // Event handlers for capturing output
        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data == null)
            {
                // Output completed for the current line
                output.AppendLine(currentLine);
                OutputReceived?.Invoke(sender, currentLine);
                currentLine = "";
            }
            else
            {
                // Append the data to the current line
                currentLine += e.Data;
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.AppendLine($"Error: {e.Data}");
                OutputReceived?.Invoke(sender, $"Error: {e.Data}");
            }
        };

        // Start the process
        process.Start();

        // Begin asynchronous read of the output
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for the process to exit
        process.WaitForExit();

        // Close the process
        process.Close();

        // Return the output as a string
        return output.ToString();
    }

}