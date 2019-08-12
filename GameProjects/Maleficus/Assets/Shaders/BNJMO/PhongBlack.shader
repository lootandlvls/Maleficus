
Shader "BNJMO/PhongBlack" {
	Properties{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shiness("Shiness", Float) = 10
		_Texture("Texture", 2D) = "White"
		_Distance("Distance", Float) = 1
		_DistanceThreshold("DistanceThreshold", Float) = 10
		_MaxDistance("MaxDistance", Float) = 15
	}

		SubShader{
		Tags{ "LightMode" = "ForwardBase" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Pass{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

		//user defined variables
		uniform float4 _Color;
	uniform float4 _SpecColor;
	uniform float _Shiness;
	uniform sampler2D _Texture;
	uniform float _Distance;
	uniform float _DistanceThreshold;
	uniform float _MaxDistance;

	//unity defined variables
	uniform float4 _LightColor0;

	//structs
	struct vertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texUV : TEXCOORD0;
	};

	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
		float2 texUV : TEXCOORD2;
	};

	// vertex function
	vertexOutput vert(vertexInput v) {
		vertexOutput o;

		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);

		o.pos = UnityObjectToClipPos(v.vertex);

		o.texUV = v.texUV;

		

		return o;
	}

	// fragment function
	float4 frag(vertexOutput i) : COLOR{
		//vectors
		float3 normalDirection = i.normalDir;
		float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);  		//substraction of 2 points in space => vector
		float3 lightDirection;
		float atten = 1.0;

		//lighting
		lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));

		//float3 diffuseReflection =   2 * ( dot( normalDirection, lightDirection ) ) * normalDirection

		float3 reflectionVector = (2 * dot(normalDirection, lightDirection) * normalDirection) - lightDirection;
		// cancel effect on backside (Lambert lightning model)
		float3 specularReflection = atten * _SpecColor.rgb * saturate(pow(max(0.0, dot(reflectionVector, viewDirection)), _Shiness))    /** max( 0.0, dot( normalDirection, lightDirection ) )*/;

		//float3 ambientLight = 0.5f * _Color;

		float3 lightFinal = diffuseReflection + specularReflection /*+ ambientLight*/ + UNITY_LIGHTMODEL_AMBIENT;

		//float4 = tex2D(_)

		float3 texCol = tex2D(_Texture, i.texUV);

		float blackFactor = 1;
		if (_Distance > _DistanceThreshold)
		{
			blackFactor = 1 - saturate((_Distance - _DistanceThreshold) / (_MaxDistance - _DistanceThreshold));
		}


		return float4(lightFinal * _Color.rgb * texCol * blackFactor, _Color.a);
	}


		ENDCG



	}
	}
}