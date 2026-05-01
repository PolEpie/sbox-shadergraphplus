
namespace ShaderGraphPlus.Nodes;

/// <summary>
/// Adjusts the contrast of a color around a gamma-correct midpoint.
/// </summary>
[Title( "Contrast" ), Category( "Artistic/Adjustment" ), Icon( "contrast" )]
public sealed class ContrastNode : ShaderNodePlus
{
	[JsonIgnore, Hide, Browsable( false )]
	public override Color NodeTitleColor => ShaderGraphPlusTheme.NodeHeaderColors.FunctionNode;

	[Hide]
	public static string ContrastFunc => @"
float3 SGP_Contrast( float3 In, float Contrast )
{
    float midpoint = pow( 0.5, 2.2 );
    return (In - midpoint) * Contrast + midpoint;
}
";

	/// <summary>
	/// Input color (RGB).
	/// </summary>
	[Input( typeof( Vector3 ) )]
	[Hide]
	public NodeInput In { get; set; }

	/// <summary>
	/// Contrast amount. 1 = unchanged, 0 = flat grey, values above 1 increase contrast.
	/// </summary>
	[Input( typeof( float ) )]
	[Hide]
	public NodeInput Contrast { get; set; }

	public float DefaultContrast { get; set; } = 1.0f;

	/// <summary>
	/// Contrast-adjusted color (RGB).
	/// </summary>
	[Output( typeof( Vector3 ) )]
	[Hide]
	public NodeResult.Func Out => ( GraphCompiler compiler ) =>
	{
		var colorIn = compiler.ResultOrDefault( In, 0.0f ).Cast( 3 );
		var contrast = compiler.ResultOrDefault( Contrast, DefaultContrast ).Cast( 1 );

		string func = compiler.RegisterHLSLFunction( ContrastFunc, "SGP_Contrast" );
		string funcCall = compiler.ResultHLSLFunction( func, $"{colorIn}, {contrast}" );

		return new NodeResult( ResultType.Vector3, funcCall );
	};
}
