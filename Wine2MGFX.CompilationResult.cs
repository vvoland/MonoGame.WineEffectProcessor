namespace WineEffectProcessor
{
    public partial class Wine2MGFX
    {
        public struct CompilationResult
        {
            public readonly bool Success;
            public readonly byte[] Compiled;
            public readonly string Error;

            public CompilationResult(byte[] compiled)
            {
                Success = true;
                Compiled = compiled;
                Error = "";
            }

            public CompilationResult(string error)
            {
                Success = false;
                Compiled = null;
                Error = error;
            }
        }
    }
}