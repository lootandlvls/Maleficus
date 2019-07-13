﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BNJMO/Faza" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10
		_RimColor ("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0

	}

	SubShader {
		Pass {
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//used defined variables
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float4 _RimColor;
			uniform float _Shininess;
			uniform float _RimPower;

			//unity defines variables
			uniform float4 _LightColor0;

			//Base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float4 col : COLOR;
			};


			//vertrex funtion
			vertexOutput vert (vertexInput v) {
				vertexOutput o;

				     
				o.posWorld = mul (unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize( mul ( float4 (v.normal, 0.0 ), unity_WorldToObject ).xyz ) ;
				o.pos = UnityObjectToClipPos (v.vertex);


				float3 normalDirection = normalize( mul ( float4 (v.normal, 0.0 ), unity_WorldToObject ).xyz ) ;
				float3 viewDirection = normalize ( _WorldSpaceCameraPos.xyz - o.posWorld.xyz );
				float3 lightDirection = normalize ( _WorldSpaceLightPos0.xyz );
				float atten = 1.0;

				//lighting
				float3 diffuseReflection = atten * _LightColor0.xyz * saturate ( dot ( normalDirection, lightDirection ) );
				float3 specularReflection = atten * _LightColor0.xyz * _SpecColor * saturate( dot ( normalDirection, lightDirection ) ) * pow( saturate( dot( reflect( -lightDirection , normalDirection ), viewDirection ) ), _Shininess );


				//Rim Lighting 
			//	float rimLighting = 1 - saturate( dot( normalize( viewDirection ), normalDirection ) ); 				// really cool effect
				float rim = 1 - saturate( dot( normalize( viewDirection ), normalDirection ) );
				float3 rimLighting = (saturate ( dot ( normalize (viewDirection) , normalDirection))) * _Color.xyz;
				float3 rimLighting2 = (1  - saturate ( dot ( normalize (viewDirection) , normalDirection) )) * _RimColor.xyz ;

				float3 lightFinal = rimLighting*0 + rimLighting2  ;

				o.col = float4( lightFinal * _Color.rgb, 1.0 );
				o.pos = UnityObjectToClipPos( v.vertex );
				return o;

			}


			//fragment funct ion
			float4 frag (vertexOutput i) : COLOR {
				


				return i.col;
			}


			ENDCG
		}
	}

	//Flabback "specular"
}