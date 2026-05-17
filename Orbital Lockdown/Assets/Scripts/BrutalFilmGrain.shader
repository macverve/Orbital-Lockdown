Shader "Hidden/Shader/BrutalFilmGrain"
{
    Properties
    {
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);

        return output;
    }

    TEXTURE2D_X(_MainTex);

    float _Intensity;
    float _GrainScale;
    float _Speed;
    float _ShadowBoost;
    float _Contrast;
    float _Colored;

    float Hash21(float2 p)
    {
        p = frac(p * float2(123.34, 456.21));
        p += dot(p, p + 45.32);
        return frac(p.x * p.y);
    }

    float3 GrainColor(float2 cell)
    {
        float r = Hash21(cell + float2(17.13, 91.77));
        float g = Hash21(cell + float2(43.71, 12.49));
        float b = Hash21(cell + float2(88.22, 67.31));
        return float3(r, g, b) * 2.0 - 1.0;
    }

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord);
        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv).rgb;

        float luminance = dot(sourceColor, float3(0.2126, 0.7152, 0.0722));
        float shadowMask = saturate((1.0 - luminance) * _ShadowBoost);

        float2 pixelCoord = uv * _ScreenSize.xy;

        float scaled = max(_GrainScale, 0.001);
        float2 cell1 = floor(pixelCoord / scaled + _Time.y * _Speed);
        float2 cell2 = floor(pixelCoord / (scaled * 0.5) - _Time.y * (_Speed * 1.37));

        float n1 = Hash21(cell1);
        float n2 = Hash21(cell2 + float2(13.0, 71.0));

        float grainMono = ((n1 * 0.65 + n2 * 0.35) * 2.0 - 1.0);
        grainMono = sign(grainMono) * pow(abs(grainMono), max(_Contrast, 0.001));

        float amount = _Intensity * (0.35 + shadowMask);

        float3 grain;
        if (_Colored > 0.5)
        {
            float3 c1 = GrainColor(cell1);
            float3 c2 = GrainColor(cell2 + float2(29.0, 53.0));
            grain = normalize(c1 * 0.65 + c2 * 0.35) * abs(grainMono);
        }
        else
        {
            grain = grainMono.xxx;
        }

        float3 finalColor = saturate(sourceColor + grain * amount * 0.20);

        return float4(finalColor, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "BrutalFilmGrain"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment CustomPostProcess
            ENDHLSL
        }
    }

    Fallback Off
}