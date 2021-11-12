Shader "Unlit/TerrainShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
		_Occupancy ("Occupancy", Float) = -1.0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"	

			uniform float _Occupancy;

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2g
            {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

			struct g2f
			{
				float4 worldPos : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

            sampler2D _MainTex;
			float3 _MainTex_ST;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			[maxvertexcount(60)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> tristream)
			{
				g2f v0, v1, v2, v0p, v1p, v2p;
				v0.worldPos = input[0].vertex;
				v0.uv = input[0].uv;
				v1.worldPos = input[1].vertex;
				v1.uv = input[1].uv;
				v2.worldPos = input[2].vertex;
				v2.uv = input[2].uv;

				v0p.worldPos = input[0].vertex;
				v0p.uv = input[0].uv;
				v0p.worldPos.y -= 0.75f;
				v1p.worldPos = input[1].vertex;
				v1p.uv = input[1].uv;
				v1p.worldPos.y -= 0.75f;
				v2p.worldPos = input[2].vertex;
				v2p.uv = input[2].uv;
				v2p.worldPos.y -= 0.75f;

				// Bottom
				tristream.Append(v0);
				tristream.Append(v1);
				tristream.Append(v2);
				//tristream.RestartStrip();

				// Top
				tristream.Append(v0p);
				tristream.Append(v1p);
				tristream.Append(v2p);
				//tristream.RestartStrip();

				// F
				tristream.Append(v2);
				tristream.Append(v2p);
				tristream.Append(v0p);

				tristream.Append(v0p);
				tristream.Append(v0);
				tristream.Append(v2);

				// F
				tristream.Append(v1);
				tristream.Append(v1p);
				tristream.Append(v2p);

				tristream.Append(v2p);
				tristream.Append(v2);
				tristream.Append(v1);
				
				// F
				tristream.Append(v0);
				tristream.Append(v0p);
				tristream.Append(v1p);

				tristream.Append(v1p);
				tristream.Append(v1);
				tristream.Append(v0);
				tristream.RestartStrip();
			}

            fixed4 frag (g2f i) : SV_Target
            {
				fixed4 col;
                // sample the texture
				if (_Occupancy > 0.0f) {
					 col = lerp(fixed4(1.0f,0.1f,0.1f,1.0f), fixed4(0.1f, 0.1f, 1.0f, 1.0f), _Occupancy);
				}
				else {
					col = tex2D(_MainTex, i.uv);
				}
				
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
