using NodeEditorPlus;

namespace ShaderGraphPlus.Nodes;

[Title( "Boolean Combo Switch" ), Category( "Utility/Logic" ), Icon( "alt_route" )]
[InternalNode]
public sealed class BooleanFeatureSwitchNode : ShaderNodePlus, IParameterNode
{
	[Hide, JsonIgnore, Browsable( false )]
	public override Color NodeTitleColor { get; set; } = ShaderGraphPlusTheme.NodeHeaderColors.LogicNode;

	[Hide, JsonIgnore, Browsable( false )]
	public override string Title => $"F_{Feature.Name.ToUpper().Replace( " ", "_" )}";

	[Hide, JsonIgnore, Browsable( false )]
	public string Name => $"F_{Feature.Name.ToUpper().Replace( " ", "_" )}";

	[Hide, JsonIgnore, Browsable( false )]
	public Guid ParameterIdentifier { get; set; }

	[Hide, JsonIgnore, Browsable( false )]
	public ShaderFeatureBoolean Feature => GetFeature();

	[Input]
	[Title( "True" )]
	[Hide]
	public NodeInput InputTrue { get; set; }

	[Input]
	[Title( "False" )]
	[Hide]
	public NodeInput InputFalse { get; set; }

	[Title( "Preview" )]
	public bool Preview { get; set; } = false;

	private ShaderFeatureBoolean GetFeature()
	{
		if ( Graph is ShaderGraphPlus graph )
		{
			var parameter = graph.FindParameter<ShaderFeatureBooleanParameter>( ParameterIdentifier );

			if ( parameter.IsValid )
			{
				var featureBoolean = new ShaderFeatureBoolean
				{
					Name = parameter.Name,
					Description = parameter.Description,
					HeaderName = parameter.HeaderName,
				};

				return featureBoolean;
			}
		}

		return null;
	}

	[Output, Hide]
	public NodeResult.Func Result => ( GraphCompiler compiler ) =>
	{
		var inputs = new List<NodeInput>
		{
			InputTrue,
			InputFalse
		};

		var result = compiler.ResultFeatureSwitch( inputs, Feature, Preview ? 1 : 0 );

		return result.IsValid ? result : new NodeResult( ResultType.Float, $"1.0f" );
	};
}
