// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BNJMO/Geometry Deformed" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shiness ("Shiness", Float) = 10
		_Texture("Texture", 2D) = "White"
	}

	SubShader {
		Tags { "LightMode" = "ForwardBase" }
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

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

				o.pos = v.vertex;

				o.texUV = v.texUV;

				return o;
			}


			// geometry function
		/*	[maxvertexcount(3)]
			void geom(triangle vertexOutput i[3], inout TriangleStream<vertexOutput> outputStream)
			{
				vertexOutput v = (vertexOutput)0;
				v.pos = float4(i[0].pos.x + 0.5, i[0].pos.y + 0.5, i[0].pos.z + 0.5, i[0].pos.w);
				v.posWorld = i[0].posWorld;
				v.normalDir = i[0].normalDir;

				outputStream.Append(v);

				v.pos = i[1].pos;
				v.posWorld = i[1].posWorld;
				v.normalDir = i[1].normalDir;

			//	v.pos = float4(i[0].pos.x - 0.5, i[0].pos.y - 0.5, i[0].pos.z - 0.5, i[0].pos.w);
				outputStream.Append(v);


				v.pos = i[2].pos;
				v.posWorld = i[2].posWorld;
				v.normalDir = i[2].normalDir;
				//v.pos = float4(i[0].pos.x, i[0].pos.y - 0.5, i[0].pos.z - 0.5, i[0].pos.w);
				outputStream.Append(v);
			}*/



			void Add(float3 pos, inout TriangleStream<vertexOutput> outputStream)
			{
				vertexOutput v = (vertexOutput)0;
				v.pos = UnityObjectToClipPos(float4(pos, 1.0));
				v.posWorld = mul(unity_ObjectToWorld, pos);

				outputStream.Append(v);
			}

			[maxvertexcount(4)]
			void geom(triangle vertexOutput i[3], inout TriangleStream<vertexOutput> outputStream)
			{
				float3 upVec = UNITY_MATRIX_IT_MV[1].xyz;
				float3 pos;

				vertexOutput v = (vertexOutput)0;
				pos = i[0].pos + float3(0.5, 0.5, 0) + upVec;
				Add(pos, outputStream);

				pos = i[0].pos + float3(-0.5, 0.5, 0) + upVec;
				Add(pos, outputStream);

				pos = i[0].pos + float3(0.5, -0.5, 0) + upVec;
				Add(pos, outputStream);

				pos = i[0].pos + float3(-0.5, -0.5, 0) + upVec;
				Add(pos, outputStream);
			}

			



			
			// fragment function
			float4 frag( vertexOutput i ) : COLOR {
				//vectors
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );  		//substraction of 2 points in space => vector
				float3 lightDirection;
				float atten = 1.0;

				//lighting
				lightDirection = normalize( _WorldSpaceLightPos0.xyz );
				float3 diffuseReflection = atten * _LightColor0.xyz * max( 0.0, dot( normalDirection, lightDirection ) );

				//float3 diffuseReflection =   2 * ( dot( normalDirection, lightDirection ) ) * normalDirection
					
				float3 reflectionVector = (2 * dot(normalDirection, lightDirection) * normalDirection) - lightDirection;
																				   						 // cancel effect on backside (Lambert lightning model)
				float3 specularReflection = atten * _SpecColor.rgb * pow( max( 0.0, dot( reflectionVector, viewDirection ) ), _Shiness)    /** max( 0.0, dot( normalDirection, lightDirection ) )*/; 
				
				//float3 ambientLight = 0.5f * _Color;

				float3 lightFinal = diffuseReflection + specularReflection /*+ ambientLight*/  + UNITY_LIGHTMODEL_AMBIENT;

				//float4 = tex2D(_)

				float3 texCol = tex2D(_Texture, i.texUV);

				return float4( lightFinal * _Color.rgb * texCol, _Color.a );
			}

			 
			ENDCG



		}
	}
}