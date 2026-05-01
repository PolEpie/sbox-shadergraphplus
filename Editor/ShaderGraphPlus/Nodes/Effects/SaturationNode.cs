
namespace ShaderGraphPlus.Nodes;

/// <summary>
/// Adjusts the saturation of a color using luminance-preserving weights.
/// </summary>
[Title( "Saturation" ), Category( "Artistic/Adjustment" ), Icon( "palette" )]
public sealed class SaturationNode : ShaderNodePlus
{
	[JsonIgnore, Hide, Browsable( false )]
	public override Color NodeTitleColor => ShaderGraphPlusTheme.NodeHeaderColors.FunctionNode;

	[Hide]
	public static string SaturationFunc => @"
float3 SGP_Saturation( float3 In, float Saturation )
{
    float luma = dot( In, float3( 0.2126729, 0.7151522, 0.0721750 ) );
    return luma.xxx + Saturation.xxx * (In - luma.xxx);
}
";

	/// <summary>
	/// Input color (RGB).
	/// </summary>
	[Input( typeof( Vector3 ) )]
	[Hide]
	public NodeInput In { get; set; }

	/// <summary>
	/// Saturation amount. 0 = greyscale, 1 = unchanged, values above 1 boost saturation.
	/// </summary>
	[Input( typeof( float ) )]
	[Hide]
	public NodeInput Saturation { get; set; }

	public float DefaultSaturation { get; set; } = 1.0f;

	/// <summary>
	/// Saturation-adjusted color (RGB).
	/// </summary>
	[Output( typeof( Vector3 ) )]
	[Hide]
	public NodeResult.Func Out => ( GraphCompiler compiler ) =>
	{
		var colorIn = compiler.ResultOrDefault( In, 0.0f ).Cast( 3 );
		var saturation = compiler.ResultOrDefault( Saturation, DefaultSaturation ).Cast( 1 );

		string func = compiler.RegisterHLSLFunction( SaturationFunc, "SGP_Saturation" );
		string funcCall = compiler.ResultHLSLFunction( func, $"{colorIn}, {saturation}" );

		return new NodeResult( ResultType.Vector3, funcCall );
	};
}
