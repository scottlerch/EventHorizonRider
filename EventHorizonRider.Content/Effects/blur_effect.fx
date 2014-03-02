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
	float4 Color;
	Color = tex2D(TextureSampler, input.TextureCordinate.xy);
	Color += tex2D(TextureSampler, input.TextureCordinate.xy + (0.01));
	Color += tex2D(TextureSampler, input.TextureCordinate.xy + (0.01));
	Color = Color / 3;

	return Color;
}

// Compile our shader
technique Technique1
{
	pass Pass1
	{
#if SM4
		PixelShader = compile ps_4_0 PixelShaderFunction();
#elif SM3
		PixelShader = compile ps_3_0 PixelShaderFunction();
#else
		PixelShader = compile ps_2_0 PixelShaderFunction();
#endif
	}
}