Shader "Caveman/Dissolve - Bottom" {
	Properties {
     _Color ("Color", Color) = (1,1,1,1)
     _FadePosition ("Fade Position", Range (-1, 1)) = 0
     _FadeRange ("Fade Height", Range (0, 1)) = 0
}
   SubShader {
     Tags {"Queue" = "Transparent"}
      Pass {
      Cull back
      Blend SrcAlpha OneMinusSrcAlpha 
            // blend based on the fragment's alpha value
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // uniform float4x4 _Object2World; 
            // automatic definition of a Unity-specific uniform parameter
   		 float _FadePosition;
   		 float _FadeRange;

         struct vertexInput {
            float4 vertex : POSITION;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output; 
 
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.position_in_world_space = input.vertex;
               // transformation of input.vertex from object 
               // coordinates to world coordinates;
               
            
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
             float dist = distance(input.position_in_world_space, 
               float4(0.0, .5, 0.0, 1.0));
               // computes the distance between the fragment position 
               // and the origin (the 4th coordinate should always be 
               // 1 for points).
 
            if (input.position_in_world_space.y < .2)
            {
               return float4(0.0, 1.0, 0.0, 0); 
                  // color near origin
            }
            else
            {
               return float4(0.3, 0.3, 0.3, 1.0); 
                  // color far from origin
            }
         }
 
         ENDCG  
      }
   }
}