// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/GPUAni"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _AniTex("Sprite Texture", 2D) = "white" {}
        _RangeY("RY", Float) = 1
        _RangeY_D("RY_D", Float) = 1
        _FrameCount("FrameCount", Int) = 1
        _DataHeight("Height", Int) = 1
        _Bound("Height", Float) = 1
        _Frame("Frame", Float) = 1

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

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed alpha: COLOR;
                fixed4 color: TEXCOORD1;
                float2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _AniTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float4 _AniTex_TexelSize;

            float _RangeY;
            float _RangeY_D;

            int _FrameCount;
            int _DataHeight;
            float _Bound;

            float _Frame;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                float3 pos = float3(v.vertex.x, v.vertex.y, 0);
                float start_time = v.normal.z;
                float offset = v.normal.y;
                float ani_length = v.normal.x;
                int w = (int)(offset * _RangeY_D);
                int h = (int)(offset - w * _RangeY);
                float b_x = w * _FrameCount + 0.5;
                float b_y = h * _DataHeight + 0.5;
                float frame_index = saturate((_Time.y - start_time) / ani_length) * _FrameCount;
                int pre = round(frame_index);
                float4 info = tex2Dlod(_AniTex, float4( (b_x + pre) * _AniTex_TexelSize.x, (b_y) * _AniTex_TexelSize.y, 0, 0));
                float3 pre_pos = info.xyz - float3(0.5, 0.5, 0.5);
                pre_pos *= _Bound * 2;
                
                pos = pos + pre_pos;
                OUT.vertex = UnityObjectToClipPos(float4(pos, 1));
                OUT.alpha = info.a;//saturate((_Time.y - start_time) / ani_length);
                OUT.color = info;
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.alpha;

                return color;
            }
        ENDCG
        }
    }
}
