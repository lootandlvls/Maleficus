// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BNJMO/Specular" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shiness ("Shiness", Float) = 10

	}

	SubShader {
		Tags { "LightMode" = "ForwardBase" }

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			//user defined variables
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shiness;
			


			//unity defined variables
			uniform float4 _LightColor0;

			//structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			// vertex function
			vertexOutput vert( vertexInput v ) {
				vertexOutput o;

				//vectors
				float3 normalDirection = normalize( mul( float4( v.normal, 0.0 ), unity_ObjectToWorld ).xyz);
				float3 viewDirection = normalize( float3( float4( _WorldSpaceCameraPos.xyz, 1.0 ) - mul (unity_ObjectToWorld, v.vertex).xyz ) );  		//substraction of 2 points in space => vector
				float3 lightDirection;
				float atten = 1.0;

				//lighting
				lightDirection = normalize( _WorldSpaceLightPos0.xyz );
				float3 diffuseReflection = atten * _LightColor0.xyz * max( 0.0, dot( normalDirection, lightDirection ) );
																													   						 // cancel effect on backside (Lambert lightning model)
				float3 specularReflection = atten * _SpecColor.rgb * pow( max( 0.0, dot( reflect (-lightDirection, normalDirection ), viewDirection ) ), _Shiness)    * max( 0.0, dot( normalDirection, lightDirection ) ); 
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;


				o.col = float4( lightFinal * _Color.rgb, 1.0 );
			
				o.pos = UnityObjectToClipPos( v.vertex );
				return o;
			}

			// fragment function
			float4 frag( vertexOutput i ) : COLOR {
				return i.col;
			}


			ENDCG



		}
	}
}