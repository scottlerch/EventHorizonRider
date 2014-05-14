#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

// Our texture sampler
texture Texture;
sampler TextureSampler = sampler_state
{
	Texture = <Texture>;
};

// This data comes from the sprite batch vertex shader
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCordinate : TEXCOORD0;
};

// Our pixel shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 c = 0;

	// Combine a number of weighted image filter taps.
	for (int i = 0; i < SAMPLE_COUNT; i++)
	{
		c += tex2D(TextureSampler, input.TextureCordinate + SampleOffsets[i]) * SampleWeights[i];
	}

	return c;
}

// Compile our shader
technique Technique1
{
	pass Pass1
	{
#if SM4
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
#elif SM3
		PixelShader = compile ps_3_0 PixelShaderFunction();
#else
		PixelShader = compile ps_2_0 PixelShaderFunction();
#endif
	}
}