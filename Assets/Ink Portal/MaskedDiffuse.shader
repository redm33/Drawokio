Shader "Caveman/Adjustable Masked Diffuse" {
 Properties {
     _Color ("Color Tint", Color) = (1,1,1,1)
     _MainTex ("Diffuse (RGB)", 2D) = "white" {}
     _MainTex2 ("Mask (RGB)", 2D) = "white" {}
          
     _Cutoff("Alpha cutoff",Range(0,1))=0.5
     
     _TexSlider ("Diffuse Fade", Range (0, 1)) = 0
     
}
 
 SubShader {
 Tags { "SHADOWSUPPORT"="true" "QUEUE"="Transparent" "RenderType"="Transparent" }
     LOD 300
 	 Pass {
        ZWrite On
        ColorMask 0
    }
 CGPROGRAM
 #pragma surface surf Lambert alpha
 
 sampler2D _MainTex;
 sampler2D _MainTex2;
 fixed4 _Color;
 float _Cutoff;
 
 // Remember to add your properties in here also. Check ShaderLab References from manual for more info
 float _TexSlider;
 float _BumpMapSlider;
 
 struct Input {
     float2 uv_MainTex;
     float2 uv_MainTex2;
     float2 uv_BumpMap;
     
    
 };
 
 void surf (Input IN, inout SurfaceOutput o) {
     fixed4 tex1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
     fixed4 tex2 = tex2D(_MainTex2, IN.uv_MainTex2) * _Color;
     float alpha = tex2D(_MainTex2, IN.uv_MainTex2).a;

     o.Albedo = tex1.rgb;
     if (alpha < _Cutoff)
   		o.Alpha = 1 - tex2.a;
	 else
   		o.Alpha = 0;

 
     // Read values from _BumpMap and _BumpMap2
//     fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
  //   normal.z = _BumpMapSlider;     
     // Interpolater between them and set the result to the o.Normal
    // o.Normal = normalize(normal);
 }
 ENDCG  
 }
 
 FallBack "Bumped Diffuse"
 }
 