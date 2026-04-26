namespace ShaderGraphPlus;

internal struct VoidFunctionOutputData : IValid
{
	public string UserAssignedName { get; private set; }
	public string CompilerAssignedName { get; private set; }
	public ResultType ResultType { get; private set; }

	public readonly bool IsValid => !string.IsNullOrWhiteSpace( UserAssignedName ) || ResultType != ResultType.Invalid;

	public VoidFunctionOutputData( string userAssignedName, string compilerAssignedName, ResultType resultType )
	{
		UserAssignedName = userAssignedName;
		CompilerAssignedName = compilerAssignedName;
		ResultType = resultType;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine( UserAssignedName, CompilerAssignedName, ResultType );
	}
}

internal struct VoidFunctionData : IValid
{
	public List<VoidFunctionOutputData> TargetResults { get; private set; }

	public string FunctionCall { get; private set; }

	/// <summary>
	/// The Identifier of the node that this data is bound to.
	/// </summary>
	public string NodeIdentifier { get; private set; }

	/// <summary>
	/// Is this void data ment for a void function thats included from a hlsl file?
	/// </summary>
	public bool IsIncludedFunction { get; private set; }

	public bool IsAlreadyPostProcessed { get; set; }

	public readonly bool IsValid => TargetResults != null && TargetResults.Any();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="targetResults"></param>
	/// <param name="functionCall"></param>
	/// <param name="nodeIdentifier"></param>
	/// <param name="isIncludedFunction"></param>
	public VoidFunctionData( List<VoidFunctionOutputData> targetResults, string functionCall, string nodeIdentifier, bool isIncludedFunction )
	{
		TargetResults = targetResults;
		FunctionCall = functionCall;
		NodeIdentifier = nodeIdentifier;
		IsIncludedFunction = isIncludedFunction;
	}

	public string ResultInit( string name, ResultType resultType )
	{
		switch ( resultType )
		{
			case ResultType.Bool:
				return $"bool {name} = false;";
			case ResultType.Int:
				return $"int {name} = 0;";
			case ResultType.Float:
				return $"float {name} = 0.0f;";
			case ResultType.Vector2:
				return $"float2 {name} = float2( 0.0f, 0.0f );";
			case ResultType.Vector3:
				return $"float3 {name} = float3( 0.0f, 0.0f, 0.0f );";
			case ResultType.Vector4:
				return $"float4 {name} = float4( 0.0f, 0.0f, 0.0f, 0.0f );";
			case ResultType.Float2x2:
				return $"float2x2 {name} = float2x2( 0.0f, 0.0f, 0.0f, 0.0f );";
			case ResultType.Float3x3:
				return $"float3x3 {name} = float3x3( 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f );";
			case ResultType.Float4x4:
				return $"float4x4 {name} = float4x4( 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f );";
			default:
				throw new NotImplementedException( $"Unknown ResultType `{resultType}`" );
		}
	}

	public ResultType GetResultResultType( string compilerAssignedName )
	{
		var result = TargetResults.Where( x => x.CompilerAssignedName == compilerAssignedName ).FirstOrDefault();

		if ( result.IsValid )
			return result.ResultType;

		throw new Exception( $"Key `{compilerAssignedName}` does not exist within `{nameof( VoidFunctionData.TargetResults )}`" );
	}

	public string GetCompilerAssignedName( string userAssignedName )
	{
		var result = TargetResults.Where( x => x.UserAssignedName == userAssignedName ).FirstOrDefault();

		if ( result.IsValid )
			return result.CompilerAssignedName;

		throw new Exception( "Shits fucked..." );
	}

	public override readonly int GetHashCode()
	{
		var hashCodeTargetResults = 0;
		foreach ( var item in TargetResults )
		{
			hashCodeTargetResults += item.GetHashCode();
		}

		return HashCode.Combine( FunctionCall, hashCodeTargetResults );
	}
}
