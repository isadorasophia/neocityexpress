#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 MatrixTransform;
sampler inputTexture;

// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
	float4 position     : SV_Position;
	float2 texCoord0    : TEXCOORD0;
	float4 color		: COLOR0;
};

VSOutput SpriteVertexShader(
	float4 position     : POSITION0,
	float2 texCoord0	: TEXCOORD0,
	float4 color		: COLOR0)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.texCoord0 = texCoord0;
	output.color = color;
	return output;
}
float Saturation;

float4 SimpleSpritePixelShader(VSOutput input) : SV_Target0
{
	float4 color = tex2D(inputTexture, input.texCoord0);

	return color * input.color;
}

technique Simple
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL SimpleSpritePixelShader();
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
	}
}
