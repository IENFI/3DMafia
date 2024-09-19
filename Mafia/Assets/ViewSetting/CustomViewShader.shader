Shader "Custom/CircularViewShader"
{
    Properties
    {
        _Radius ("View Radius", Float) = 10.0
        _Center ("View Center", Vector) = (0.5, 0.5, 0, 0)
        _FadeDistance ("Fade Distance", Float) = 2.0
        _DarkColor ("Darkness Color", Color) = (0, 0, 0, 1)  // 어두운 영역의 색상
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Radius;
            float _FadeDistance;
            float4 _Center;
            float4 _DarkColor;  // 어두운 영역의 색상 (검정 또는 원하는 색)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.worldPos;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 플레이어 중심으로부터의 거리 계산
                float dist = distance(i.worldPos.xy, _Center.xy);

                // 시야 범위 밖이면 어둡게 만듦
                if (dist > _Radius)
                {
                    float fade = (dist - _Radius) / _FadeDistance;
                    fade = clamp(fade, 0.0, 1.0);
                    return lerp(half4(1, 1, 1, 1), _DarkColor, fade); // 시야 밖으로 갈수록 어둡게
                }

                return half4(1, 1, 1, 1); // 시야 안에서는 정상적으로 표시
            }
            ENDCG
        }
    }
}
