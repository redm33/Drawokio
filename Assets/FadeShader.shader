﻿Shader "Caveman/Dissolve - Bottom" {
	Properties {
     _Color ("Color", Color) = (1,1,1,1)
     _FadePosition ("Fade Position", Range (0, 1)) = 0
     _FadeRange ("Fade Height", Range (0, 1)) = 0
     _ObjectPosition ("Object Y Position", Float) = 0
     _OutlineColor ("Outline Color", Color) = (1,1,1,1)
	 _Outline ("Outline width", Range (0.0, 0.03)) = .005
}


CGINCLUDE
#include "UnityCG.cginc"
#include "AutoLight.cginc"

 float4 _Color;
 float _FadePosition;
 float _FadeRange;
 float _ObjectPosition;
 uniform float _Outline;
 uniform float4 _OutlineColor;
 
struct appdataOutline {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct appdataDiffuse {
	float4 vertex : POSITION;
};

struct v2fOutline{
	float4 pos : POSITION;
    float4 localPosition : TEXCOORD0;
	float4 color : COLOR;
};

struct v2fDiffuse {
	float4 pos : SV_POSITION;
    float4 localPosition : TEXCOORD0;
    LIGHTING_COORDS(1,2)
};





ENDCG
 
SubShader {
Tags {"SHADOWSUPPORT"="true" "Queue" = "Transparent+1" "RenderType"="Transparent"}
	LOD 300
	Pass {
    	Cull Back
    	Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting On
 
        CGPROGRAM
        #pragma vertex vert  
        #pragma fragment frag 
		v2fDiffuse vert(appdataDiffuse input) {
			v2fDiffuse output; 
    		output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
    		output.localPosition = mul(_Object2World, input.vertex);
    		TRANSFER_VERTEX_TO_FRAGMENT(o);
    		return output;
		}
        
        float4 frag(v2fDiffuse input) : COLOR {
         	float atten = LIGHT_ATTENUATION(input);

        	if (input.localPosition.y < _FadePosition + _ObjectPosition ) {
            	return float4(_Color.r, _Color.g, _Color.b, 0) * atten; 
            }
            else if (input.localPosition.y < (_FadePosition + _ObjectPosition) + _FadeRange && _FadeRange > 0 && _FadePosition > 0) {
            	return float4(_Color.r, _Color.g, _Color.b, (input.localPosition.y - (_FadePosition + _ObjectPosition))/_FadeRange) * atten;
            }
            else {
            	return float4(_Color.r, _Color.g, _Color.b, 1.0) * atten; 
            }
         }
         ENDCG  
      }
      
	Pass {
		Name "BASE"
		Tags { "LightMode" = "Always" }
		Cull Back
		Blend Zero One
		// uncomment this to hide inner details:
		//Offset -8, -8
		SetTexture [_OutlineColor] {
			ConstantColor (0,0,0,1)
			Combine constant
		}
	}
		// note that a vertex shader is specified here but its using the one above
	Pass {
		Name "OUTLINE"
		Tags { "LightMode" = "Always" }
		Cull Front
 		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting On
 
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		v2fOutline vert(appdataOutline v) {
			// just make a copy of incoming vertex data but scaled according to normal direction
			v2fOutline o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 			o.localPosition = mul(_Object2World, v.vertex);;
    		TRANSFER_VERTEX_TO_FRAGMENT(o);
	
			float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
			float2 offset = TransformViewToProjection(norm.xy);
 	
			o.pos.xy += offset * o.pos.z * _Outline;
			o.color = _OutlineColor;
			return o;
		}
		
 		half4 frag(v2fOutline input) :COLOR {

	
			if (input.localPosition.y < _FadePosition + _ObjectPosition ) {
            	return float4(input.color.r, input.color.g, input.color.b, 0); 
            }
            else if (input.localPosition.y < (_FadePosition + _ObjectPosition) + _FadeRange && _FadeRange > 0 && _FadePosition > 0) {
            	return float4(input.color.r, input.color.g, input.color.b, (input.localPosition.y - (_FadePosition + _ObjectPosition))/_FadeRange);
            }
            else {
            	return float4(input.color.r, input.color.g, input.color.b, 1.0); 
            }
			
		}
		ENDCG
	}
	
}
Fallback "Diffuse"
}