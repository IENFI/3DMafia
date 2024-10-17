Shader "Custom/XRayShader"
{
    Properties
    {
        _XRayColor ("X-Ray Color", Color) = (1,0,0,1)
        _XRayIntensity ("X-Ray Intensity", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        LOD 100

        // 첫 번째 패스: 깊이 작성을 비활성화하고 항상 렌더링
        Pass
        {
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            fixed4 _XRayColor;
            float _XRayIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normalDir = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float rim = 1.0 - saturate(dot(viewDir, normalDir));
                fixed4 col = _XRayColor * pow(rim, 1.5) * _XRayIntensity;
                col.a = rim * _XRayIntensity;
                return col;
            }
            ENDCG
        }
    }
}