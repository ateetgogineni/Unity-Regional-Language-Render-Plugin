Shader "Custom/UIOutlineUniformNoTint"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            Name "OUTLINE_PASS"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float centerAlpha = tex2D(_MainTex, i.uv).a;

                float pixelScale = _OutlineSize / _ScreenParams.y;

                float outlineAlpha = 0.0;
                for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    float2 offset = float2(x, y) * pixelScale;
                    outlineAlpha += tex2D(_MainTex, i.uv + offset).a;
                }

                float4 baseColor = tex2D(_MainTex, i.uv); // No tint applied
                float threshold = 0.05;

                if (centerAlpha > threshold)
                {
                    return baseColor;
                }
                else if (outlineAlpha > threshold)
                {
                    return float4(_OutlineColor.rgb, _OutlineColor.a);
                }
                else
                {
                    return float4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
}
