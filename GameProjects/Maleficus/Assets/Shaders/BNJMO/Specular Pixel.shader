// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BNJMO/SpecularPixel" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shiness ("Shiness", Float) = 10
		
	}

	SubShader {
		Tags { "LightMode" = "ForwardBase" }
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			//user defined variables
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shiness;
			uniform sampler2D _Texture;

			//unity defined variables
			uniform float4 _LightColor0;

			//structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				
			};

			// vertex function
			vertexOutput vert( vertexInput v ) {
				vertexOutput o;

				o.posWorld = mul (unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize( mul( float4( v.normal, 0.0 ), transpose(unity_WorldToObject) ).xyz);

				o.pos = UnityObjectToClipPos (v.vertex);
				
				//float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);
			//	o.pos = o.pos = mul(UNITY_MATRIX_MVP, saturate(normalize(v.vertex * dot(viewDirection, v.vertex))));
				return o;
			}
			
			// fragment function
			float4 frag( vertexOutput i ) : COLOR {
				//vectors
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );  		//substraction of 2 points in space => vector
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				//float3 reflectionDirection = (2 * dot(normalDirection, lightDirection) * normalDirection) - lightDirection;
				float3 reflectionDirection = reflect(-lightDirection, normalDirection);
				
				float atten = 1.0;

				float3 diffuseReflection = atten * _LightColor0.xyz * max( 0.0, dot( normalDirection, lightDirection ) );

				//float3 diffuseReflection =   2 * ( dot( normalDirection, lightDirection ) ) * normalDirection
					
				
																				   															 // cancel effect on backside (Lambert lightning model)
				float3 specularReflection = atten * _SpecColor.rgb * pow( max( 0.0, dot( reflectionDirection, viewDirection ) ), _Shiness)    /** max( 0.0, dot( normalDirection, lightDirection ) )*/;
				
				//float3 ambientLight = 0.5f * _Color;

				float3 lightFinal = diffuseReflection + specularReflection /*+ ambientLight*/  + UNITY_LIGHTMODEL_AMBIENT;

				//float4 = tex2D(_)

				

				return float4( lightFinal * _Color.rgb , _Color.a );
			}

			 
			ENDCG



		}
	}
}