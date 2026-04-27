using static ShaderGraphPlus.ShaderGraphPlusGlobals;

namespace ShaderGraphPlus.Nodes;

[Title( "Enum Combo Switch" ), Category( "Utility/Logic" ), Icon( "alt_route" )]
[InternalNode]
public sealed class EnumFeatureSwitchNode : ShaderNodePlus, BaseNodePlus.IInitializeNode, IParameterNode, IErroringNode
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
	public ShaderFeatureEnum Feature => GetFeature();

	[global::Editor( ControlWidgetCustomEditors.ShaderFeatureEnumPreviewIndexEditor )]
	[Title( "Preview" )]
	public int PreviewIndex { get; set; } = 0;

	[Hide]
	private List<IPlugIn> InternalInputs = new();

	[Hide]
	public override IEnumerable<IPlugIn> Inputs => InternalInputs;

	[Hide, JsonIgnore]
	int _lastHashCodeInputs = 0;

	//[Hide, JsonIgnore]
	//bool _hasFeatureError = false;

	public override void OnFrame()
	{
		var hashCodeInput = Feature.GetHashCode();
		if ( hashCodeInput != _lastHashCodeInputs )
		{
			//var oldHashCode = _lastHashCodeInputs;
			_lastHashCodeInputs = hashCodeInput;

			//SGPLog.Info( $"HashCode changed from : {oldHashCode} to {_lastHashCodeInputs}" );

			// Dont update or change if feature is not valid!
			if ( Feature.IsValid )
			{
				CreateInputs();
				Update();
			}
		}
	}

	private ShaderFeatureEnum GetFeature()
	{
		if ( Graph is ShaderGraphPlus graph )
		{
			var parameter = graph.FindParameter<ShaderFeatureEnumParameter>( ParameterIdentifier );

			if ( parameter.IsValid )
			{
				var featureEnum = new ShaderFeatureEnum
				{
					Name = parameter.Name,
					Description = parameter.Description,
					HeaderName = parameter.HeaderName,
					Options = parameter.Options,
				};

				return featureEnum;
			}
		}

		return null;
	}

	[Output, Hide]
	public NodeResult.Func Result => ( GraphCompiler compiler ) =>
	{
		var inputs = new List<NodeInput>();

		foreach ( var input in Inputs )
		{
			if ( input.ConnectedOutput is null )
			{
				NodeInput nodeInput = default;

				inputs.Add( nodeInput );
			}
			else
			{
				NodeInput nodeInput = new NodeInput { Identifier = input.ConnectedOutput.Node.Identifier, Output = input.ConnectedOutput.Identifier };

				inputs.Add( nodeInput );
			}
		}

		var result = compiler.ResultFeatureSwitch( inputs, Feature, PreviewIndex );

		return result.IsValid ? result : new NodeResult( ResultType.Float, $"1.0f" );
	};

	public void InitializeNode()
	{
		OnNodeCreated();
	}

	private void OnNodeCreated()
	{
		CreateInputs();
		Update();
	}

	public void CreateInputs()
	{
		var inPlugs = new List<IPlugIn>();

		if ( Feature.Options == null )
		{
			InternalInputs = new();
		}
		else
		{
			foreach ( var input in Feature.Options )
			{
				var inputName = input;
				// Default to float.
				var inputType = typeof( float );//typeof( object );

				if ( string.IsNullOrWhiteSpace( inputName ) ) continue;

				var info = new PlugInfo()
				{
					Name = inputName,
					Type = inputType,
					DisplayInfo = new DisplayInfo()
					{
						Name = inputName,
						Fullname = inputType.FullName
					}
				};

				var plug = new BasePlugIn( this, info, inputType );
				var oldPlug = InternalInputs.FirstOrDefault( x => x is BasePlugIn plugIn && plugIn.Info.Name == info.Name && plugIn.Info.Type == info.Type ) as BasePlugIn;
				if ( oldPlug is not null )
				{
					oldPlug.Info.Name = info.Name;
					oldPlug.Info.Type = info.Type;
					oldPlug.Info.DisplayInfo = info.DisplayInfo;
					plug = oldPlug;
				}

				inPlugs.Add( plug );
			}

			InternalInputs = inPlugs;
		}
	}

	public List<string> GetErrors()
	{
		var errors = new List<string>();

		//foreach ( var option in Feature.Options )
		//{
		//	if ( string.IsNullOrWhiteSpace( option ) )
		//	{
		//		errors.Add( $"element \"{Feature.Options.IndexOf( option )}\" of feature \"{Feature.Name}\" cannot have a blank name!" );
		//	}
		//}

		return errors;
	}
}
