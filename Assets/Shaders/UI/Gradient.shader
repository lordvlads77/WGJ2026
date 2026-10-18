Shader "UI/Gradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Gradient)]
        [NoScaleOffset] _GradientTex ("Gradient Ramp", 2D) = "white" {}
        [Enum(Linear,0,Radial,1,Angular,2)] _GradientType ("Type", Float) = 0
        _GradientAngle ("Angle", Range(0, 360)) = 0
        _GradientCenter ("Center (XY)", Vector) = (0.5, 0.5, 0, 0)
        _GradientScale ("Scale", Float) = 1
        _GradientOffset ("Offset", Float) = 0
        [Enum(Clamp,0,Repeat,1,PingPong,2)] _GradientWrap ("Wrap Mode", Float) = 0
        [Toggle] _KeepAspect ("Keep Aspect", Float) = 1
        _SpriteColorBlend ("Sprite Color Blend", Range(0, 1)) = 0

        [Header(UI)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 gradientData : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4  color : COLOR;
                float2 uv : TEXCOORD0;
                float4 gradientData : TEXCOORD1;
                float4 positionOS : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GradientTex);
            SAMPLER(sampler_GradientTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4  _Color;
                float  _GradientType;
                float  _GradientAngle;
                float4 _GradientCenter;
                float  _GradientScale;
                float  _GradientOffset;
                float  _GradientWrap;
                float  _KeepAspect;
                half   _SpriteColorBlend;
            CBUFFER_END

            float4 _ClipRect;
            half4  _TextureSampleAdd;

            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionOS = v.positionOS;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.gradientData = v.gradientData;
                o.color = v.color * _Color;
                return o;
            }

            float ComputeGradientT(float2 p, float aspect)
            {
                float  ax = _KeepAspect > 0.5 ? aspect : 1.0;
                float2 q  = (p - _GradientCenter.xy) * float2(ax, 1.0);
                float  a  = radians(_GradientAngle);
                float  t;

                if (_GradientType < 0.5)
                {
                    float2 dir = float2(cos(a), sin(a));
                    float extent = 0.5 * (abs(dir.x) * ax + abs(dir.y));
                    t = dot(q, dir) / max(extent, 1e-4) * 0.5 + 0.5;
                }
                else if (_GradientType < 1.5)
                {
                    t = length(q) * 2.0;
                }
                else
                {
                    float angle = atan2(q.y, q.x) - a;
                    t = frac(angle / TWO_PI + 1.0);
                }

                t = t / max(_GradientScale, 1e-4) + _GradientOffset;

                if (_GradientWrap < 0.5) t = saturate(t);                           
                else if (_GradientWrap < 1.5) t = frac(t);                               
                else t = 1.0 - abs(frac(t * 0.5) * 2.0 - 1.0);  

                return t;
            }

            half4 frag(Varyings i) : SV_Target
            {
                bool   hasRectData = i.gradientData.z > 0.0;
                float2 p      = hasRectData ? i.gradientData.xy : i.uv;
                float  aspect = hasRectData ? i.gradientData.z : 1.0;

                float t = ComputeGradientT(p, aspect);
                half4 gradient = SAMPLE_TEXTURE2D_LOD(_GradientTex, sampler_GradientTex, float2(t, 0.5), 0);
                half4 sprite   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) + _TextureSampleAdd;

                half4 col = gradient * i.color;
                col.a   *= sprite.a;
                col.rgb *= lerp(half3(1, 1, 1), sprite.rgb, _SpriteColorBlend);

                #ifdef UNITY_UI_CLIP_RECT
                float2 inside = step(_ClipRect.xy, i.positionOS.xy) * step(i.positionOS.xy, _ClipRect.zw);
                col.a *= inside.x * inside.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}