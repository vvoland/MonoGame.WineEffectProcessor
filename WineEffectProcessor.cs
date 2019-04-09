using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace WineEffectProcessor
{
    [ContentProcessor(DisplayName = "Effect Processor - Wine")]
    public sealed class WineEffectProcessor : EffectProcessor
    {
        private readonly Wine2MGFX Mgfx;

        public WineEffectProcessor()
        {
            Mgfx = new Wine2MGFX(
                "/ext/wine/monogame/drive_c/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/2MGFX.exe",
                "/usr/bin/wine64",
                "/ext/wine/monogame/");
        }

        public override CompiledEffectContent Process(EffectContent input, ContentProcessorContext context)
        {
            if(Environment.OSVersion.Platform != PlatformID.Unix)
                return base.Process(input, context);

            string hlslCode = ExpandIncludes(input.EffectCode);
            byte[] compiled = Compile(context.TargetPlatform, input.Identity, hlslCode);

            return new CompiledEffectContent(compiled);
        }

        private byte[] Compile(TargetPlatform targetPlatform, ContentIdentity identity, string hlslCode)
        {
            Wine2MGFX.CompilationResult result = Mgfx.Compile(hlslCode, targetPlatform,
                DebugMode == EffectProcessorDebugMode.Debug,
                Defines);

            if (!result.Success)
            {
                string error = result.Error
                    .Replace(Wine2MGFX.ShaderFilePlaceholder, identity.SourceFilename);

                throw new InvalidContentException(error);
            }

            return result.Compiled;
        }


        private static string ExpandIncludes(string hlsl)
        {
            Regex include = new Regex(@"#include ""([a-zA-Z0-9.\/]+)""");
            return include.Replace(hlsl, match =>
            {
                try
                {
                    if (match.Success)
                    {
                        if (match.Groups.Count > 1)
                        {
                            string filename = match.Groups[1].Value;
                            return File.ReadAllText(filename);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // If file is not found, do not replace directive
                }

                return match.Value;
            });
        }
    }
}