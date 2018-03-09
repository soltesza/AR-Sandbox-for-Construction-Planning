Shader "Unlit/RoadShader" {
	Properties {
		_ColorCut("ColorCut", Color) = (1., 0., 0.)
		_ColorFill("ColorFill", Color) = (0., 0., 1.)
	}

	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
		LOD 100

		ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog // make fog work

			#include "UnityCG.cginc"

			float4 _ColorCut, _ColorFill;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				float y_pos : BLENDWEIGHT1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.y_pos = v.vertex.y;
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				float y = i.y_pos;

				if (y <= .5)
					return _ColorCut;

				return _ColorFill;
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				//return col;
			}
			ENDCG
		}
	}
}
