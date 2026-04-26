
namespace ShaderGraphPlus;

public sealed partial class GraphCompiler
{
	private partial class CompileResult
	{
		public Dictionary<string, VoidFunctionData> VoidFunctionLocals { get; private set; } = new();
	}
}
