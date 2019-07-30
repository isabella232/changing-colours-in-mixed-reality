// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Rainbow shader with lots of adjustable properties!

Shader "Custom/HueGradient" {
	Properties{
		_Saturation("Saturation", Range(-1.0, 1.0)) = 0.0
		_Luminosity("Luminosity", Range(0.0, 1.0)) = 0.5
    _Target("Target", Range(-0.5, 0.5)) = 0.5
		_Spread("Spread", Range(0.0, 1.0)) = 1.0
    _Hue("Hue", Range(0.0, 1.0)) = 0.0
	}
		SubShader{
		Pass{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Shared/ShaderTools.cginc"

  fixed _Saturation;
	fixed _Luminosity;
	half _Spread;
  half _Target;
  half _Hue;

	struct vertexInput {
		float4 vertex : POSITION;
		float4 texcoord0 : TEXCOORD0;
	};

	struct fragmentInput {
		float4 position : SV_POSITION;
		float4 texcoord0 : TEXCOORD0;
		fixed3 localPosition : TEXCOORD1;
	};

	fragmentInput vert(vertexInput i) {
		fragmentInput o;
		o.position = UnityObjectToClipPos(i.vertex);
		o.texcoord0 = i.texcoord0;
		o.localPosition = i.vertex.xyz; +fixed3(0.5, 0.5, 0.5);
		return o;
	}

	fixed4 frag(fragmentInput i) : SV_TARGET{
    if (i.texcoord0.x > 0.5 - _Spread / 2 && i.texcoord0.x < 0.5 + _Spread / 2) {
		  fixed4 hsl = fixed4(-i.texcoord0.x - _Target + 1.0 + _Hue, _Saturation + 1.0, _Luminosity, 1.0);
		  return HSLtoRGB(hsl);
    }
    return fixed4(0.0, 0.0, 0.0, 0.0);
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
}