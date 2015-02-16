  Shader "Example/Slices" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Off
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
	  
	  struct Input {
		float2 uv_MainTex;
		float3 localPos;
	  };
	  
	  void vert (inout appdata_full v, out Input o) {
		  UNITY_INITIALIZE_OUTPUT(Input,o);
		  o.localPos = v.vertex.xyz;
		  o.Alpha = o.localPos.y / 100;
	  }
      
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          //clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5);
          o.Alpha = IN.localPos.y/100;
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }