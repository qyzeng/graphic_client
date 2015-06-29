Shader "Transparent Effects/CheapForcefield" {
Properties
 {
     _Color("_Color", Color) = (0,1,0,1)
     _Inside("_Inside", Range(0,0.2) ) = 0
     _Rim("_Rim", Range(1,2) ) = 1.2
     _Texture("_Texture", 2D) = "white" {}
     _XSpeed("_XSpeed", Float ) = 0
     _YSpeed("_YSpeed", Float ) = 0
     _Tile("_Tile", Range(1,10) ) = 5
     _Strength("_Strength", Range(0,5) ) = 1.5
 }
    
 SubShader
 {
     Tags
     {
         "Queue"="Transparent"
         "RenderType"="Transparent"
 
    }
 
       
	 Cull Off
	 ZWrite On
	 ZTest LEqual
 
		
		CGPROGRAM
		#pragma surface surf BlinnPhong alpha
		#include "UnityStandardBRDF.cginc"

		fixed4 _Color;
		fixed _Inside;
		fixed _Rim;
		sampler2D _Texture;
		fixed _XSpeed;
		fixed _YSpeed;
		fixed _Tile;
		fixed _Strength;

		struct Input {
			float4 screenPos;
			float3 viewDir;
			float2 uv_Texture;
		};

		inline fixed Fresnel(float3 viewDir)
		{
			float3 up = float3(0.0f,0.0f,1.0f);
			
			float theta = dot( Unity_SafeNormalize(viewDir), up);
			
			return fixed(1.0f - theta);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// calculate Fresnel
			fixed fresnel = Fresnel( IN.viewDir );
			
			// fresnel related effects
			fixed stepfresnel = step( fresnel , fixed(1.0f));
			fixed insideContrib = clamp( stepfresnel , _Inside , fixed(1.0f));
			fixed rimContrib = pow( fresnel , _Rim );
			
			// calculate texture coords
			half xtimeOffset = half(_Time.x) * _XSpeed;
			half ytimeOffset = half(_Time.x) * _YSpeed;
			half2 texCoords = float2( IN.uv_Texture.x + xtimeOffset, IN.uv_Texture.y + ytimeOffset );
			texCoords *= _Tile.xx;
			
			// and get the texture contribution
			fixed  texContrib = tex2D (_Texture, texCoords).x;
			texContrib *= _Strength;
			
			// put the contributions together into the alpha
			o.Alpha = texContrib * rimContrib * insideContrib * _Color.a;
			
			// set the colours
			o.Albedo = 0.0;
			o.Emission = _Color.rgb;
			o.Normal = fixed3(0.0,0.0,1.0);
		}
		ENDCG
	} 
	 Fallback "Diffuse"
}