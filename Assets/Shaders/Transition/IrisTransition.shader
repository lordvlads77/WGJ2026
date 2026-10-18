Shader "UI/IrisTransition"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Silhouette (Alpha)", 2D) = "white" {}
        _Scale ("Scale (screen heights)", Float) = 3
        _Aspect ("Screen Aspect", Float) = 1.7777
        _MaskAspect ("Mask Aspect", Float) = 1
        _Softness ("Edge Softness", Range(0, 0.5)) = 0.03

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MaskTex;
            float _Scale;
            float _Aspect;
            float _MaskAspect;
            float _Softness;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.texcoord - 0.5;
                p.x *= _Aspect;

                float s = max(_Scale, 1e-4);
                float2 maskUV = float2(p.x / (s * _MaskAspect), p.y / s) + 0.5;
                float inside = step(0.0, maskUV.x) * step(maskUV.x, 1.0) * step(0.0, maskUV.y) * step(maskUV.y, 1.0);
                float m = tex2D(_MaskTex, maskUV).a * inside;
                float soft = max(_Softness, 0.001);
                m = smoothstep(0.5 - soft, 0.5 + soft, m);

                fixed4 col = i.color;
                col.a *= 1.0 - m;

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif
                return col;
            }
            ENDCG
        }
    }
}