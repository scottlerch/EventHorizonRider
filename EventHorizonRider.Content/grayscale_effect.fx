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
	float4 color = tex2D(TextureSampler, input.TextureCordinate);

	float value = 0.299*color.r + 0.587*color.g + 0.114*color.b;
	color.r = value;
	color.g = value;
	color.b = value;
	color.a = 1.0f;

	return color;
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