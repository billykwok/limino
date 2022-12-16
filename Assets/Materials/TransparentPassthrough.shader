Shader "MixedReality/TransparentPassthrough" {
  Properties {
    _InvertedAlpha("Inverted Alpha", float) = 0

    [Header(DepthTest)]
    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
    [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpColor("Blend Color", Float) = 2 //"ReverseSubtract"
    [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpAlpha("Blend Alpha", Float) = 3 //"Min"
  }
  SubShader {
    Tags {
      "RenderType"="Transparent"
    }
    LOD 100

    Pass {
      ZWrite Off
      ZTest[_ZTest]
      BlendOp[_BlendOpColor], [_BlendOpAlpha]
      Blend Zero One, One One

      HLSLPROGRAM
      // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members center)
      //#pragma exclude_renderers d3d11
      #pragma vertex vert
      #pragma fragment frag

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL;
      };

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      float _InvertedAlpha;

      v2f vert(appdata v) {
        v2f o;
        o.vertex = TransformObjectToHClip(v.vertex);
        o.uv = v.uv;
        return o;
      }

      half4 frag(v2f i) : SV_Target {
        return half4(0, 0, 0, 1 - _InvertedAlpha);
      }
      ENDHLSL
    }
  }
}
