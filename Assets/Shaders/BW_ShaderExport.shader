Shader "UI/MaskableSaturation"
{
    Properties
    {
        [PerRendererData]
        _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        // 0 = grayscale
        // 1 = original color
        _Controller ("Saturation", Range(0,1)) = 1

        // UI Mask / Stencil properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        // ============================================================
        // STENCIL
        // This is what allows Unity's Mask component to work.
        // ============================================================

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        // ============================================================
        // UI RENDER STATE
        // ============================================================

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UI"

            HLSLPROGRAM

            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            // --------------------------------------------------------
            // UI keywords
            // --------------------------------------------------------

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            // --------------------------------------------------------
            // Unity includes
            // --------------------------------------------------------

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            // --------------------------------------------------------
            // Properties
            // --------------------------------------------------------

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            float _Controller;

            // Used by RectMask2D
            float4 _ClipRect;

            // --------------------------------------------------------
            // Vertex
            // --------------------------------------------------------

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;

                // Required by RectMask2D
                float4 worldPosition : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = v.vertex;

                OUT.vertex = UnityObjectToClipPos(v.vertex);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Unity UI vertex color is important:
                // Image.color, CanvasGroup alpha, etc.
                OUT.color = v.color * _Color;

                return OUT;
            }

            // ========================================================
            // SATURATION
            // ========================================================

            float3 ApplySaturation(float3 color, float saturation)
            {
                float luminance = dot(
                    color,
                    float3(
                        0.2126729,
                        0.7151522,
                        0.0721750
                    )
                );

                return lerp(
                    luminance.xxx,
                    color,
                    saturation
                );
            }

            // ========================================================
            // FRAGMENT
            // ========================================================

            fixed4 frag(v2f IN) : SV_Target
            {
                // ----------------------------------------------------
                // Sample Image
                // ----------------------------------------------------

                fixed4 color = tex2D(
                    _MainTex,
                    IN.texcoord
                );

                // ----------------------------------------------------
                // Apply Unity UI vertex color
                // ----------------------------------------------------

                color *= IN.color;

                // ----------------------------------------------------
                // Apply saturation ONLY to RGB
                // ----------------------------------------------------

                color.rgb = ApplySaturation(
                    color.rgb,
                    saturate(_Controller)
                );

                // ----------------------------------------------------
                // RectMask2D
                //
                // This is Unity's normal UI clipping mechanism.
                // ----------------------------------------------------

                #ifdef UNITY_UI_CLIP_RECT

                    color.a *= UnityGet2DClipping(
                        IN.worldPosition.xy,
                        _ClipRect
                    );

                #endif

                // ----------------------------------------------------
                // Optional alpha clipping
                // ----------------------------------------------------

                #ifdef UNITY_UI_ALPHACLIP

                    clip(color.a - 0.001);

                #endif

                return color;
            }

            ENDHLSL
        }
    }

    FallBack "UI/Default"
}