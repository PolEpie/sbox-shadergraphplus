using ShaderGraphPlus.Nodes;
using System.Text.Json.Nodes;

namespace ShaderGraphPlus;

public partial class ShaderGraphPlus
{
	[SGPJsonUpgrader( typeof( ShaderGraphPlus ), 7 )]
	internal static void Upgrader_v7( JsonObject obj )
	{
		if ( !CheckIfSubgraph( obj ) )
			return;

		//
		// Upgrade Parameters
		//

		if ( obj[JsonKeys.ParameterArray] is not JsonArray oldParameterArray )
			return;

		var newParameterArray = new JsonArray();

		foreach ( var jsonNode in oldParameterArray )
		{
			if ( jsonNode[JsonKeys.Class] is not JsonValue classValue )
				continue;

			var parameterElement = JsonSerializer.Deserialize<JsonElement>( jsonNode.AsObject().ToJsonString() );
			var typeName = classValue.GetValue<string>();

			if ( typeName == "ShaderFeatureEnumParameter" )
			{
				var updatedParameterObject = jsonNode.DeepClone().AsObject();

				if ( updatedParameterObject.ContainsKey( "Options" ) )
				{
					var options = updatedParameterObject["Options"].Deserialize<List<string>>( SerializerOptions() );
					var newOptions = new List<ShaderFeatureEnumOption>();

					foreach ( var option in options )
					{
						newOptions.Add( new ShaderFeatureEnumOption() { Name = option } );
					}

					updatedParameterObject.Remove( "Options" );
					updatedParameterObject["Options"] = JsonSerializer.SerializeToNode( newOptions, SerializerOptions() );
				}
				else
				{
					updatedParameterObject["Options"] = JsonSerializer.SerializeToNode( new List<ShaderFeatureEnumOption>(), SerializerOptions() );
				}

				newParameterArray.Add( updatedParameterObject );
			}
			else
			{
				newParameterArray.Add( jsonNode.DeepClone() );
			}
		}


		//
		// Upgrade Nodes
		//

		if ( obj[JsonKeys.NodeArray] is not JsonArray oldNodeArray )
			return;

		var newNodeArray = new JsonArray();

		foreach ( var jsonNode in oldNodeArray )
		{
			if ( jsonNode[JsonKeys.Class] is not JsonValue classValue )
				continue;

			var nodeElement = JsonSerializer.Deserialize<JsonElement>( jsonNode.AsObject().ToJsonString() );
			var typeName = classValue.GetValue<string>();

			if ( typeName == "SubgraphOutput" )
			{
				var updatedNodeObject = jsonNode.DeepClone().AsObject();

				if ( GetDeserializedKey<Guid>( updatedNodeObject, "OutputIdentifier", out var parameterIdentifier ) )
				{
					updatedNodeObject["ParameterIdentifier"] = JsonSerializer.SerializeToNode( parameterIdentifier, SerializerOptions() );
				}
				else
				{
					parameterIdentifier = Guid.NewGuid();
				}

				if ( GetDeserializedKey<string>( updatedNodeObject, "OutputName", out var outputName ) )
				{
				}
				else
				{
					throw new Exception( $"Cannot find key with the name : 'OutputName'" );
				}
		;
				if ( GetDeserializedKey<string>( updatedNodeObject, "OutputDescription", out var outputDescription ) )
				{
				}
				else
				{
					outputDescription = "";
				}

				if ( GetDeserializedKey<SubgraphPortType>( updatedNodeObject, "OutputType", out var outputType ) )
				{
				}
				else
				{
					throw new Exception( $"Cannot find key with the name : 'OutputType'" );
				}

				if ( GetDeserializedKey<SubgraphOutputPreviewType>( updatedNodeObject, "Preview", out var preview ) )
				{
				}
				else
				{
					preview = SubgraphOutputPreviewType.None;
				}

				if ( GetDeserializedKey<int>( updatedNodeObject, "PortOrder", out var portOrder ) )
				{
				}
				else
				{
					portOrder = 0;
				}

				IBlackboardSubgraphOutputParameter parameter;

				switch ( outputType )
				{
					case SubgraphPortType.Bool:
						parameter = new BoolSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Int:
						parameter = new IntSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Float:
						parameter = new FloatSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Vector2:
						parameter = new Float2SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Vector3:
						parameter = new Float3SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Vector4:
						parameter = new Float4SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Color:
						parameter = new ColorSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Float2x2:
						parameter = new Float2x2SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Float3x3:
						parameter = new Float3x3SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Float4x4:
						parameter = new Float4x4SubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Gradient:
						parameter = new GradientSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.SamplerState:
						parameter = new SamplerStateSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.Texture2DObject:
						parameter = new Texture2DSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					case SubgraphPortType.TextureCubeObject:
						parameter = new TextureCubeSubgraphOutputParameter()
						{
							Identifier = parameterIdentifier
						};

						break;
					default:
						throw new NotImplementedException( $"Unknown OutputType \"{outputType}\"" );
				}
				;

				parameter.Name = outputName;
				parameter.OutputDescription = outputDescription;
				parameter.Preview = preview;
				parameter.PortOrder = portOrder;

				var parameterType = parameter.GetType();
				var parameterObject = new JsonObject { { JsonKeys.Class, parameterType.Name } };

				SerializeObject( parameter, parameterObject, SerializerOptions() );

				newParameterArray.Add( parameterObject );

				newNodeArray.Add( updatedNodeObject );
			}
			else
			{
				newNodeArray.Add( jsonNode.DeepClone() );
			}
		}

		obj.Remove( JsonKeys.ParameterArray );
		obj.Add( JsonKeys.ParameterArray, newParameterArray );

		obj.Remove( JsonKeys.NodeArray );
		obj.Add( JsonKeys.NodeArray, newNodeArray );
	}

}
