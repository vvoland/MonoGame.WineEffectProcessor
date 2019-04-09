MonoGame Pipeline Content Processor plugin which falls back to launching Windows version of 2MGFX through wine to compile effects on Unix-like platforms.

Shaders are copied into C:\Temp directory inside Wine Prefix so that the compiler does not need to touch 'host' file system.

0. Install MonoGame for Visual studio inside wine prefix
1. Do `winetricks d3dcompiler_47` inside desired wine prefix
2. Build project and add **WineEffectProcessor.dll** to Content references
3. Set effect processor to **Effect Processor - Wine**