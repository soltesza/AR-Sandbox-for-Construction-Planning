Shader "Unlit/RoadControlPointShader"
{
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay"}
        LOD 200

        ZTest Always

        Pass {
            Tags { "LightMode" = "Always" }

            Fog { Mode Off }
            ZWrite On

            Cull Back
            Lighting Off

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest

                fixed4 _Color;

                struct appdata {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float4 vertex : POSITION;
                };

                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                fixed4 frag (v2f i) : COLOR {
                    return _Color;
                }
            ENDCG

        }
    }
}
