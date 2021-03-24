
uniform float _progress;
uniform float4 _ball;
uniform float _wave1_speed;
uniform float _wave2_speed;
uniform float _wave1_frequency;
uniform float _wave2_frequency;
uniform float _wave1_amplitude;
uniform float _wave2_amplitude;
uniform float _wave_offset;
uniform float _wave_brightness;
uniform float _gscale;

inline float2 CaculateBallFluidMask(float3 world_pos)
{
    float radius = _ball.w * _gscale;
    float x = world_pos.x - _ball.x + radius;
    float wave1 = sin((x * 0.05 / _gscale + _Time.y * _wave1_speed) * _wave1_frequency) * 0.5f + 0.5f;
    float wave2 = sin((x * 0.05 / _gscale + _Time.y * _wave2_speed + _wave_offset) * _wave2_frequency) * 0.5f + 0.5f;

    float wave1_width = radius * _wave1_amplitude;
    float wave2_width = radius * _wave2_amplitude;

    float max_width = max(wave1_width, wave2_width);
    float v = 1 - _progress;
    float offset = v * (radius * 2) + max_width * 2 * (v - 0.5);
    float y = world_pos.y - _ball.y + radius + offset;

    float wave1_mask = (radius * 2 - (y - wave1 * wave1_width)) / wave1_width;
    float wave2_mask = (radius * 2 - (y - wave2 * wave2_width)) / wave2_width;

    float dis = distance(world_pos, _ball.xyz);
    float ball_mask = saturate((radius - dis) / radius * 100.0f);

    float plus = saturate(wave1_mask) * saturate(-wave2_mask);
    plus = (1 + _wave_brightness * saturate(plus * 2));

    float mask = saturate(wave1_mask * 10) + saturate(wave2_mask * 10);
    float final_mask = saturate(ball_mask * mask);

    return float2(final_mask, plus);
}