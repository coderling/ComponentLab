Shader "Long/Effect/WorldSpaceUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Clip("Clip", range(0, 1)) = 0
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float3 w_pos: TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal:NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Clip;

            v2f vert (appdata v)
            {
                v2f o;
                o.w_pos =  mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                o.vertex = mul(UNITY_MATRIX_VP, float4(o.w_pos, 1.0));
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(1, 1, 1, 1);
                clip(col.a - _Clip);
                return col;
            }
            ENDCG
        }
    }
}
