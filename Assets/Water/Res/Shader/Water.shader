Shader "Fluid/Water"
{
    Properties
    {
    }

    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType"="Transparent"}

        pass
        {
            Tags {"LightMode"="ForwardBase"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Water.hlsl"
            ENDHLSL
        }
    }
}