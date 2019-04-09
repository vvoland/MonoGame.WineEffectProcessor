MonoGame Pipeline Content Processor plugin which falls back to launching Windows version of 2MGFX through wine to compile effects on Unix-like platforms.

Shaders are copied into C:\Temp directory inside Wine Prefix so that the compiler does not need to touch 'host' file system.

# Install
0. Run `setup-wine-prefix.sh` to create new wine prefix and install MonoGame and d3dcompiler_47 in it (you can also do it manually if you want, just remember to change paths in WineEffectProcessor constructor)
1. Build the solution and reference **WineEffectProcessor.dll** in Content Pipeline
2. Set Processor of all Effect assets to **Effect Processor - Wine**
