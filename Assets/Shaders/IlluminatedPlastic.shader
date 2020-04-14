Shader "IOX/IlluminatedPlastic" 
{
	Properties
	{
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MVIA", 2D) = "white" {}
		_NormalsTex("Normals", 2D) = "white" {}
		_Power("Glossiness", Float) = 64
		_Shine("Shine", Float) = 20
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionStrength("Emission Strength", Float) = 4
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"LightMode" = "Deferred"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			#pragma target 3.0

			struct Input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float3 color : COLOR;
			};

			struct v2f
			{
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
				float3 bitangent : BITANGENT;
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 color : COLOR;
			};

			v2f vert(Input v)
			{
				v2f o;
				o.normal = mul(unity_ObjectToWorld, v.normal);
				o.tangent = mul(unity_ObjectToWorld, v.tangent.xyz);
				o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			struct FragmentOutput 
			{
				float4 gBuffer0 : SV_Target0;
				float4 gBuffer1 : SV_Target1;
				float4 gBuffer2 : SV_Target2;
				float4 gBuffer3 : SV_Target3;
			};

			sampler2D _MainTex;
			sampler2D _NormalsTex;
			half _Power;
			half _Shine;
			half3 _Color;
			half _Cutoff;
			half3 _EmissionColor;
			half _EmissionStrength;

			FragmentOutput frag(v2f i)
			{
				half4 ctex = tex2D(_MainTex, i.uv);
				clip(ctex.a - _Cutoff);

				half4 ntex = tex2D(_NormalsTex, i.uv);
				half3 n = ntex.xyz * 2 - 1;
				half3 normal = n.x * i.tangent + n.y * i.bitangent + n.z * i.normal;

				half3 emission = ctex.b * _EmissionColor;
				emission = exp2(-emission.rgb * _EmissionStrength);

				FragmentOutput output;
				output.gBuffer0 = float4(lerp(half3(1, 1, 1), _Color * i.color, ctex.r) * ctex.g, _Shine / 64);
				output.gBuffer1 = float4(0, 0, 0, _Power / 256);
				output.gBuffer2 = float4(normal * .5 + .5 , 0);
				output.gBuffer3 = float4(emission, 0);

				return output;
			}

			ENDCG
		}
	}
		
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"LightMode" = "ForwardBase"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			#pragma target 3.0

			struct Input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(Input v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half _Cutoff;
			half3 _Color;

			half4 frag(v2f i) : COLOR
			{
				half4 ctex = tex2D(_MainTex, i.uv);
				clip(ctex.a - _Cutoff);
				return half4(lerp(half3(1, 1, 1), _Color, ctex.r) * ctex.g, 1);
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}