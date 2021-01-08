
sampler2D animation_tex;

struct appdata_t
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    float2 offset : TEXCOORD1;
};

struct VOut
{
    float alpha : TEXCOORD2;
};

VOut vert(appdata_t v)
{
    VOut o;
    float4 offset = tex2Dlod(animation_tex, float4(0, 0, 0, 0));
    return o;
}