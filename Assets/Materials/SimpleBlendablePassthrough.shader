Shader "MixedReality/SimpleBlendablePassthrough" {
  Properties {
    _Inflation("Inflation", Float) = 0.0
    _InvertedAlpha("Inverted Alpha", Float) = 1.0

    [Header(DepthTest)]
    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4.0 //"LessEqual"
    [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpColor("Blend Color", Float) = 2.0 //"ReverseSubtract"
    [Enum(UnityEngine.Rendering.BlendOp)] _BlendOpAlpha("Blend Alpha", Float) = 3.0 //"Min"

    [MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
    [MainColor] _BaseColor("Base Color", Color) = (0, 0, 0, 0)

    _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
    _SmoothnessSource("Smoothness Source", Float) = 0.0
    [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
    _BumpScale("Scale", Float) = 1.0

    [HideInInspector] _SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
    [HideInInspector] _SpecGlossMap("Specular Map", 2D) = "white" {}
    [HideInInspector] _SpecularHighlights("Specular Highlights", Float) = 1.0

    [HideInInspector] _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.0

    [HideInInspector] [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
    [HideInInspector] [NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}

    // Blending state
    [HideInInspector] _Surface("__surface", Float) = 0.0
    [HideInInspector] _Blend("__blend", Float) = 0.0
    [HideInInspector] _Cull("__cull", Float) = 2.0
    [HideInInspector] [ToggleUI] _AlphaClip("__clip", Float) = 0.0
    [HideInInspector] _SrcBlend("__src", Float) = 1.0
    [HideInInspector] _DstBlend("__dst", Float) = 0.0
    [HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
    [HideInInspector] _DstBlendAlpha("__dstA", Float) = 0.0
    [HideInInspector] _ZWrite("__zw", Float) = 1.0
    [HideInInspector] _BlendModePreserveSpecular("_BlendModePreserveSpecular", Float) = 1.0
    [HideInInspector] _AlphaToMask("__alphaToMask", Float) = 0.0

    [ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0
  }

  SubShader {
    Tags {
      "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" "IgnoreProjector" = "True" "ShaderModel"="4.5"
    }
    LOD 300

    Pass {
      Name "Passthrough"

      ZWrite Off
      ZTest[_ZTest]
      BlendOp[_BlendOpColor], [_BlendOpAlpha]
      Blend Zero One, One One

      CGPROGRAM
      // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members center)
      //#pragma exclude_renderers d3d11
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL;
      };

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _Inflation;
      float _InvertedAlpha;

      v2f vert(appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _Inflation);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }

      fixed4 frag(v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv);
        float alpha = lerp(col.r, 1 - col.r, _InvertedAlpha);
        return float4(0, 0, 0, alpha);
      }
      ENDCG
    }

    Pass {
      Name "ForwardLit"
      Tags {
        "LightMode" = "UniversalForward"
      }

      // Use same blending / depth states as Standard shader
      Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
      ZWrite[_ZWrite]
      Cull[_Cull]
      AlphaToMask[_AlphaToMask]

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local _NORMALMAP
      #pragma shader_feature_local_fragment _EMISSION
      #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
      #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
      #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Universal Pipeline keywords
      #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
      #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
      #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
      #pragma multi_compile _ SHADOWS_SHADOWMASK
      #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
      #pragma multi_compile_fragment _ _SHADOWS_SOFT
      #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
      #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
      #pragma multi_compile_fragment _ _LIGHT_LAYERS
      #pragma multi_compile_fragment _ _LIGHT_COOKIES
      #pragma multi_compile _ _FORWARD_PLUS
      #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile _ DIRLIGHTMAP_COMBINED
      #pragma multi_compile _ LIGHTMAP_ON
      #pragma multi_compile _ DYNAMICLIGHTMAP_ON
      #pragma multi_compile_fog
      #pragma multi_compile_fragment _ DEBUG_DISPLAY
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma instancing_options renderinglayer
      #pragma multi_compile _ DOTS_INSTANCING_ON

      #pragma vertex LitPassVertexSimple
      #pragma fragment LitPassFragmentSimple
      #define BUMP_SCALE_NOT_SUPPORTED 1

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "ShadowCaster"
      Tags {
        "LightMode" = "ShadowCaster"
      }

      ZWrite On
      ZTest LEqual
      ColorMask 0
      Cull[_Cull]

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON

      // -------------------------------------
      // Universal Pipeline keywords

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
      #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

      #pragma vertex ShadowPassVertex
      #pragma fragment ShadowPassFragment

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "GBuffer"
      Tags {
        "LightMode" = "UniversalGBuffer"
      }

      ZWrite[_ZWrite]
      ZTest LEqual
      Cull[_Cull]

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      //#pragma shader_feature _ALPHAPREMULTIPLY_ON
      #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
      #pragma shader_feature_local _NORMALMAP
      #pragma shader_feature_local_fragment _EMISSION
      #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

      // -------------------------------------
      // Universal Pipeline keywords
      #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
      //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
      //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
      #pragma multi_compile_fragment _ _SHADOWS_SOFT
      #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
      #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile _ DIRLIGHTMAP_COMBINED
      #pragma multi_compile _ LIGHTMAP_ON
      #pragma multi_compile _ DYNAMICLIGHTMAP_ON
      #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
      #pragma multi_compile _ SHADOWS_SHADOWMASK
      #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
      #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma instancing_options renderinglayer
      #pragma multi_compile _ DOTS_INSTANCING_ON

      #pragma vertex LitPassVertexSimple
      #pragma fragment LitPassFragmentSimple
      #define BUMP_SCALE_NOT_SUPPORTED 1

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitGBufferPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "DepthOnly"
      Tags {
        "LightMode" = "DepthOnly"
      }

      ZWrite On
      ColorMask R
      Cull[_Cull]

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      #pragma vertex DepthOnlyVertex
      #pragma fragment DepthOnlyFragment

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
      ENDHLSL
    }

    // This pass is used when drawing to a _CameraNormalsTexture texture
    Pass {
      Name "DepthNormals"
      Tags {
        "LightMode" = "DepthNormals"
      }

      ZWrite On
      Cull[_Cull]

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      #pragma vertex DepthNormalsVertex
      #pragma fragment DepthNormalsFragment

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local _NORMALMAP
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
      // Universal Pipeline keywords
      #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitDepthNormalsPass.hlsl"
      ENDHLSL
    }

    // This pass it not used during regular rendering, only for lightmap baking.
    Pass {
      Name "Meta"
      Tags {
        "LightMode" = "Meta"
      }

      Cull Off

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      #pragma vertex UniversalVertexMeta
      #pragma fragment UniversalFragmentMetaSimple
      #pragma shader_feature EDITOR_VISUALIZATION

      #pragma shader_feature_local_fragment _EMISSION
      #pragma shader_feature_local_fragment _SPECGLOSSMAP

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "Universal2D"
      Tags {
        "LightMode" = "Universal2D"
      }
      Tags {
        "RenderType" = "Transparent" "Queue" = "Transparent"
      }

      HLSLPROGRAM
      #pragma exclude_renderers gles gles3 glcore
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
      ENDHLSL
    }
  }

  SubShader {
    Tags {
      "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" "IgnoreProjector" = "True" "ShaderModel"="2.0"
    }
    LOD 300

    Pass {
      Name "Passthrough"

      ZWrite Off
      ZTest[_ZTest]
      BlendOp[_BlendOpColor], [_BlendOpAlpha]
      Blend Zero One, One One

      CGPROGRAM
      // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members center)
      //#pragma exclude_renderers d3d11
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL;
      };

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _Inflation;
      float _InvertedAlpha;

      v2f vert(appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _Inflation);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }

      fixed4 frag(v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv);
        float alpha = lerp(col.r, 1 - col.r, _InvertedAlpha);
        return float4(0, 0, 0, alpha);
      }
      ENDCG
    }

    Pass {
      Name "ForwardLit"
      Tags {
        "LightMode" = "UniversalForward"
      }

      // Use same blending / depth states as Standard shader
      Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
      ZWrite[_ZWrite]
      Cull[_Cull]
      AlphaToMask[_AlphaToMask]

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      // DOTS instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON
      #pragma target 3.5 DOTS_INSTANCING_ON

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local _NORMALMAP
      #pragma shader_feature_local_fragment _EMISSION
      #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
      #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
      #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Universal Pipeline keywords
      #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
      #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
      #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
      #pragma multi_compile _ SHADOWS_SHADOWMASK
      #pragma multi_compile_fragment _ _SHADOWS_SOFT
      #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
      #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
      #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
      #pragma multi_compile_fragment _ _LIGHT_COOKIES
      #pragma multi_compile _ _FORWARD_PLUS


      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile _ DIRLIGHTMAP_COMBINED
      #pragma multi_compile _ LIGHTMAP_ON
      #pragma multi_compile _ DYNAMICLIGHTMAP_ON
      #pragma multi_compile_fog
      #pragma multi_compile_fragment _ DEBUG_DISPLAY
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing

      #pragma vertex LitPassVertexSimple
      #pragma fragment LitPassFragmentSimple
      #define BUMP_SCALE_NOT_SUPPORTED 1

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "ShadowCaster"
      Tags {
        "LightMode" = "ShadowCaster"
      }

      ZWrite On
      ZTest LEqual
      ColorMask 0
      Cull[_Cull]

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      // DOTS instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON
      #pragma target 3.5 DOTS_INSTANCING_ON

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Universal Pipeline keywords

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
      #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing

      #pragma vertex ShadowPassVertex
      #pragma fragment ShadowPassFragment

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "DepthOnly"
      Tags {
        "LightMode" = "DepthOnly"
      }

      ZWrite On
      ColorMask R
      Cull[_Cull]

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      // DOTS instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON
      #pragma target 3.5 DOTS_INSTANCING_ON

      #pragma vertex DepthOnlyVertex
      #pragma fragment DepthOnlyFragment

      // Material Keywords
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
      ENDHLSL
    }

    // This pass is used when drawing to a _CameraNormalsTexture texture
    Pass {
      Name "DepthNormals"
      Tags {
        "LightMode" = "DepthNormals"
      }

      ZWrite On
      Cull[_Cull]

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      // DOTS instancing
      #pragma multi_compile _ DOTS_INSTANCING_ON
      #pragma target 3.5 DOTS_INSTANCING_ON

      #pragma vertex DepthNormalsVertex
      #pragma fragment DepthNormalsFragment

      // -------------------------------------
      // Material Keywords
      #pragma shader_feature_local _NORMALMAP
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

      // -------------------------------------
      // Unity defined keywords
      #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitDepthNormalsPass.hlsl"
      ENDHLSL
    }

    // This pass it not used during regular rendering, only for lightmap baking.
    Pass {
      Name "Meta"
      Tags {
        "LightMode" = "Meta"
      }

      Cull Off

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      #pragma vertex UniversalVertexMeta
      #pragma fragment UniversalFragmentMetaSimple

      #pragma shader_feature_local_fragment _EMISSION
      #pragma shader_feature_local_fragment _SPECGLOSSMAP
      #pragma shader_feature EDITOR_VISUALIZATION

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"
      ENDHLSL
    }

    Pass {
      Name "Universal2D"
      Tags {
        "LightMode" = "Universal2D"
      }
      Tags {
        "RenderType" = "Transparent" "Queue" = "Transparent"
      }

      HLSLPROGRAM
      #pragma only_renderers gles gles3 glcore d3d11
      #pragma target 2.0

      #pragma vertex vert
      #pragma fragment frag
      #pragma shader_feature_local_fragment _ALPHATEST_ON
      #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

      #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
      ENDHLSL
    }
  }

  Fallback "Hidden/Universal Render Pipeline/FallbackError"
//  CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.SimpleLitShader"
}
