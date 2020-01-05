
struct vInput
{
    float4 vertex : POSITION;
};

struct vOutput
{
    float4 pos : SV_POSITION;
};

#define MAX_WAVE_COUNT 4
#define Gravity 9.18f
#define TWO_PI 6.28318530718f

//由多少个波进行叠加
half _DirectionalWaveCount;

//波方向或圆形波的中心
float4 _WaveDirOrCenter[MAX_WAVE_COUNT];

//波长
float4 _WaveLength;

//振幅
float4 _Amplitude;


//所有实现都在Shader中了，方便阅读，实际应该在CPU计算一次然后传到Shader中
float caculate_wave(float2 pos, float amplitude, float2 dir, float length, float time)
{
    //频率，角速度
    float freq = TWO_PI / length;
    //相位
    float speed = 1 / sqrt(length / TWO_PI * Gravity);

    float phase = speed * time;
    phase = fmod(phase, TWO_PI);
    return 2 * amplitude *  pow(sin(dot(dir, pos) * freq + phase) / 2 + 0.5, 2.5);
}

inline float2 GetWaveDir(int index, float2 pos)
{
    float3 dirOrPos = _WaveDirOrCenter[index];
    return dirOrPos.xy - pos * dirOrPos.z;
}

float caculate_height(float2 pos)
{
    float h = 0;
    for(int i = 0; i < _DirectionalWaveCount; ++i)
    {
        h += caculate_wave(pos, _Amplitude[i], GetWaveDir(i, pos), _WaveLength[i], _Time.y);
    }

    return h;
}

vOutput vert(vInput v)
{
    vOutput o;
    float3 pos = v.vertex.xyz;
    pos.y = caculate_height(pos.xz);
    o.pos = UnityObjectToClipPos(pos);
    return o;
}

float4 frag(vOutput i) : SV_TARGET
{
    return 1;
}