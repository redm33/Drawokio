Shader "Caveman/Mask" {
	Properties {
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		_Mask ("Mask (RGB)", 2D) = "white" {}
	}
	SubShader {
	    Tags {Queue=Transparent}

		Lighting On
		ZWrite off
		Blend Zero SrcColor
		
		Pass {
			SetTexture [_Mask] {
                combine texture
            }
            // Blend in the alpha texture using the lerp operator
            SetTexture [_MainTex] {
                combine texture lerp (texture) previous
            }
		}
	 }
}
