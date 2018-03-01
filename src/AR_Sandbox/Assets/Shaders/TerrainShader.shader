Shader "Unlit/TerrainShader"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}

		_Color0("Color0", Color) = (1., 0., 0.)
		_Color1("Color1", Color) = (1., 1., 0.)
		_Color2("Color2", Color) = (0., 1., 0.)
		_Color3("Color3", Color) = (0., 1., 1.)
		_Color4("Color4", Color) = (0., 0., 1.)

	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Stencil {
				Ref 1
				Comp equal
			}

			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag

			float4 _Color0;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color4;

		    struct vertInput {
		        float4 pos : POSITION;
		    };  

		    struct vertOutput {
				float4 pos : SV_POSITION;
				float wpos_y : BLENDWEIGHT1;
		    };

		    vertOutput vert(vertInput input) {
		        vertOutput o;
		        o.pos = UnityObjectToClipPos(input.pos);
				o.wpos_y = input.pos.y;
		        return o;
		    }

		    half4 frag(vertOutput input) : COLOR {
				float4 output;
				
				//color coded heights
				float y = input.wpos_y * 5;

				if (y <= 1) {
					output = lerp(_Color0, _Color1, frac(y));	//blend between colors 0 and 1
				}
				else if (y <= 2) {
					output = lerp(_Color1, _Color2, frac(y));	//blend between colors 1 and 2
				}
				else if (y <= 3) {
					output = lerp(_Color2, _Color3, frac(y));	//blend between colors 2 and 3
				}
				else {
					output = lerp(_Color3, _Color4, frac(y));	//blend between colors 3 and 4
				}

				return output;
		    }
		    ENDCG
		}
	}
}
