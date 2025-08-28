Shader "Hidden/TexturePainterDecal"
{
    Properties
    {
        _DecalTex("Decal Tex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _BrushSize("BrushSize", float) = 0.1
        _Hardness("Hardness", float) = 1.0
        _Opacity("Opacity", float) = 1.0
        _UVPos("UVPos", Vector) = (0.5,0.5,0,0)
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
            float _BrushSize;
            float _Hardness;
            float _Opacity;
            float4 _UVPos;
            sampler2D _DecalTex;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                // フルスクリーンクアッド
                float2 quad[3] = { float2(-1,-1), float2(3,-1), float2(-1,3) }; 
                // o.pos = float4(quad[uint(v.vertex.x)].xy,0,1);
                o.pos = float4(quad[id].xy, 0, 1);
                // UVは同じく0-1で割り当て
                // 頂点番号で単純対応: v.vertex.x=0->0, v.vertex.x=1->1, ...
                // ここでは単純に(-1,-1),(3,-1),(-1,3)というトライアングルからUVを計算
                // 中心固定で行うなら固定の0..1で問題なし
                // 簡易的に(uv = (pos+1)/2)でOK
                float2 uv = (o.pos.xy + 1.0) * 0.5;
                o.uv = uv;
                return o;   
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // _UVPos.xyを中心としたブラシ適用範囲を計算
                float dx = i.uv.x - _UVPos.x;
                float dy = i.uv.y - _UVPos.y;
                float dist = sqrt(dx*dx+dy*dy);

                // ブラシの形：円形を想定
                float edge = _BrushSize*0.5;
                float t = saturate((edge - dist) / (edge));
                t = pow(t, _Hardness*10); // Hardnessでエッジ強調

                if (t <= 0.001) discard;

                fixed4 c = tex2D(_DecalTex, i.uv);
                c *= _Color;
                c.a *= t * _Opacity;

                return c;
            }
            ENDCG
        }
    }
}