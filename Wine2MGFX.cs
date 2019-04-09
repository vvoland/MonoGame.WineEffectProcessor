using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace WineEffectProcessor
{
    public partial class Wine2MGFX
    {
        public static readonly string ShaderFilePlaceholder = "%%%SHADER_SOURCE%%%";

        private readonly string WinePrefix;
        private readonly string WineBinary;
        private readonly string MgfxBinary;

        private readonly string TempDir;

        public Wine2MGFX(string wineMgfxBinary, string wineBin, string winePrefix)
        {
            if (winePrefix == null)
            {
                throw new ArgumentNullException(
                    winePrefix,
                    "Wine prefix must be set. If not sure, try /home/<your_name>/.wine");
            }

            WinePrefix = winePrefix;
            WineBinary = wineBin;
            MgfxBinary = wineMgfxBinary;

            TempDir = "C:\\Temp";
        }

        public CompilationResult Compile(string source, TargetPlatform platform, bool debug = false, string defines = null)
        {
            string hostTempPath = Path.Combine(WinePrefix, "drive_c", "Temp");
            Directory.CreateDirectory(hostTempPath);

            string id = Guid.NewGuid().ToString();
            string winPath = $"{TempDir}\\{id}";
            string winInputFile = $"{winPath}.fx";
            string winOutputFile = $"{winPath}.fxo";

            string hostInputFile = Path.ChangeExtension(Path.Combine(hostTempPath, id), ".fx");
            string hostOutputFile = Path.ChangeExtension(hostInputFile, ".fxo");

            File.WriteAllText(hostInputFile, source);

            Process process = Run(winInputFile, winOutputFile, GetProfile(platform), debug, defines);
            File.Delete(hostInputFile);

            if (process.ExitCode == 0 && File.Exists(hostOutputFile))
            {
                byte[] compiled = File.ReadAllBytes(hostOutputFile);
                File.Delete(hostOutputFile);

                return new CompilationResult(compiled);
            }

            string error = process.StandardError.ReadToEnd()
                .Replace(winInputFile, ShaderFilePlaceholder)
                .Replace(EscapeBackslashes(winInputFile), ShaderFilePlaceholder);

            return new CompilationResult(error);
        }

        private string EscapeBackslashes(string path)
        {
            return path.Replace(@"\", @"\\");
        }

        private string GetProfile(TargetPlatform targetPlatform)
        {
            switch (targetPlatform)
            {
                case TargetPlatform.Android:
                case TargetPlatform.DesktopGL:
                case TargetPlatform.MacOSX:
                case TargetPlatform.iOS:
                case TargetPlatform.RaspberryPi:
                    return "OpenGL";
                default:
                    return "DirectX_11";
            }
        }

        private Process Run(string inputFile, string outputFile, string profile, bool debug, string defines)
        {
            string arguments = $"\"{inputFile}\" \"{outputFile}\" /Profile:{profile}";
            if (debug)
                arguments += " /Debug";
            if (!string.IsNullOrEmpty(defines))
                arguments += $" /Defines:{defines}";

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = WineBinary,
                    Arguments = $"\"{MgfxBinary}\" {arguments}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            process.StartInfo.EnvironmentVariables.Add("WINEPREFIX", WinePrefix);

            process.Start();
            process.WaitForExit();
            return process;
        }
    }
}