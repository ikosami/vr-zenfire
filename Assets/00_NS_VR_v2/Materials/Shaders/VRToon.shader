Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSmooth ("Shadow Smooth", Range(0, 0.5)) = 0.05
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0, 10)) = 3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float4 _Color;
            float _ShadowThreshold;
            float _ShadowSmooth;
            float4 _RimColor;
            float _RimPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * _Color;
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                float NdotL = dot(normal, lightDir);
                float shadow = smoothstep(_ShadowThreshold - _ShadowSmooth, 
                                        _ShadowThreshold + _ShadowSmooth, 
                                        NdotL);
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float rim = 1.0 - saturate(dot(viewDir, normal));
                rim = pow(rim, _RimPower);
                
                // 最終カラーの計算
                float3 finalColor = col.rgb * shadow;
                finalColor += _RimColor.rgb * rim;
                
                return float4(finalColor, col.a);
            }
            ENDCG
        }
    }
}