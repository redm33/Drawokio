Shader "Caveman/Dissolve - Bottom" {
	Properties {
     _Color ("Color", Color) = (1,1,1,1)
     _FadePosition ("Fade Position", Range (0, 1)) = 0
     _FadeRange ("Fade Height", Range (0, 1)) = 0
     _ObjectPosition ("Object Y Position", Float) = 0
}
   SubShader {
     Tags {"Queue" = "Transparent"}
      Pass {
      Cull back
      Blend SrcAlpha OneMinusSrcAlpha 
       
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
 		 float4 _Color;
   		 float _FadePosition;
   		 float _FadeRange;
   		 float _ObjectPosition;

         struct vertexInput {
            float4 vertex : POSITION;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 localPosition : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output; 
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.localPosition = mul(_Object2World, input.vertex);;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
            if (input.localPosition.y < _FadePosition + _ObjectPosition )
            {
                return float4(_Color.r, _Color.g, _Color.b, 0); 
            }
            else if (input.localPosition.y < (_FadePosition + _ObjectPosition) + _FadeRange && _FadeRange > 0 && _FadePosition > 0)
            {
            	return float4(_Color.r, _Color.g, _Color.b, (input.localPosition.y - (_FadePosition + _ObjectPosition))/_FadeRange);
            }
            else {
            	return float4(_Color.r, _Color.g, _Color.b, 1.0); 
            }
         }
 
         ENDCG  
      }
   }
}