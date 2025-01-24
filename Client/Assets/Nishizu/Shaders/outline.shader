Shader "Unlit/outline"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front  // カメラから見て表面をカリング（描かない）

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL; // 入力セマンティクスに法線を追加
                 // uvは削除
            };

            struct v2f
            {
                 // uvは削除
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // Textureは削除

            v2f vert (appdata v)
            {
                v2f o;
                // 頂点に16分の1した法線を足す（法線方向に頂点を動かしている）
                o.vertex = UnityObjectToClipPos(v.vertex + float4(v.normal.xyz, 0) / 32.0);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // rgbaで黒色を指定する（アルファ（透明度）は１）
                fixed4 col = fixed4(255,255,0,1);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
