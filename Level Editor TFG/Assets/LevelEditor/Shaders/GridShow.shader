
Shader "LevelEditor/GridShow"
{
    Properties
	{
      		_GridSizeX("Grid X Size", Float) = 1
			_GridSizeY("Grid Y Size", Float) = 1
      		_Alpha ("Alpha", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

         float _GridSizeX;
		 float _GridSizeY;
         float _Alpha;

	struct appdata
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float2 local : TEXCOORD1;
	};

	
	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = mul(unity_ObjectToWorld, v.vertex).xz;
		o.local = v.vertex.xz;
		return o;
	}



         float DrawGrid(float2 uv, float sz,float szy , float aa)
         {
            float aaThresh = aa;
            float aaMin = aa*0.1;

            float gUV1 = uv.x / sz + aaThresh;
			float gUv2 = uv.y / szy + aaThresh;
            float2 gUV = float2(gUV1,gUv2);


            float2 fl = floor(gUV);
            gUV = frac(gUV);
            gUV -= aaThresh;
            gUV = smoothstep(aaThresh, aaMin, abs(gUV));
            float d = max(gUV.x, gUV.y);

            return d;
         }

			fixed4 frag (v2f i) : SV_Target
			{              
				fixed r = DrawGrid(i.local, _GridSizeX, _GridSizeY,0.03);
				return float4(0.8*r,0,0,_Alpha);
			}
			ENDCG
		}
	}
    
}
