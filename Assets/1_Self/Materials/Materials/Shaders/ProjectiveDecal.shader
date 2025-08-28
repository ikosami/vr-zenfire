Shader "Hidden/ProjectiveDecal"
{
    Properties
    {
        _DecalTex("Decal Tex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Hardness("Hardness", float) = 1.0
        _Opacity("Opacity", float) = 1.0
        _DecalSize("DecalSize", Vector) = (0.5,0.5,0,0)
        // _ProjectorMatrix("ProjectorMatrix", Matrix) = (1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1)
        // _ProjectorMatrix("ProjectorMatrix", Matrix) = (1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1)
    }
    SubShader
    {
        Tags{"Queue"="Transparent"}
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            float _Hardness;
            float _Opacity;
            float4 _DecalSize;
            float4x4 _ProjectorMatrix;
            sampler2D _DecalTex;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float  valid : TEXCOORD1; 
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 localPos = v.vertex;
                float4 projPos = mul(_ProjectorMatrix, localPos);

                float2 uv;
                uv.x = projPos.x / _DecalSize.x + 0.5;
                uv.y = projPos.y / _DecalSize.y + 0.5;

                // 有効判定
                float valid = (projPos.z > 0 && uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1) ? 1.0 : 0.0;

                // 位置はメッシュ頂点をそのままNDCとして使用(単純化)
                o.pos = UnityObjectToClipPos(localPos);
                o.uv = uv;
                o.valid = valid;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (i.valid < 0.5) discard;

                float2 uv = i.uv;
                fixed4 c = tex2D(_DecalTex, uv);

                float dx = uv.x - 0.5;
                float dy = uv.y - 0.5;
                float dist = sqrt(dx*dx+dy*dy);
                float radius = 0.5;
                float t = saturate((radius - dist) / radius);
                t = pow(t, _Hardness * 10);

                if (t <= 0.001) discard;

                c *= _Color;
                c.a *= t * _Opacity;

                return c;
            }
            ENDCG
        }
    }
}
