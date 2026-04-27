using ShaderGraphPlus.Nodes;
using System.Text.Json.Nodes;

namespace ShaderGraphPlus;

public partial class ShaderGraphPlus
{
	[SGPJsonUpgrader( typeof( ShaderGraphPlus ), 7 )]
	internal static void Upgrader_v7( JsonObject obj )
	{
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

		obj.Remove( JsonKeys.ParameterArray );
		obj.Add( JsonKeys.ParameterArray, newParameterArray );
	}

}
