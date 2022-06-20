#include "Fractals.hlsli"

struct ShaderConstancs
{
	float Width;
	float Height;
	uint  Frame;
};

ConstantBuffer<ShaderConstancs>			shaderParams	: register(b1);
#define SAMPLES 4

float4 FillColorPS(in noperspective float4 Position : SV_Position,
	in noperspective float2 UV : TEXCOORD) : SV_Target0
{
	const float offset = 0.2f;
	const float2 offsets[SAMPLES] =
	{
		float2(-offset, offset),
		float2(offset, offset),
		float2(offset, -offset),
		float2(-offset, -offset)
	};

	const float2 invDim = float2(1.f/shaderParams.Width, 1.f/shaderParams.Height);

	float3 color = 0.f;
	for (int i = 0; i < SAMPLES; i++)
	{
		const float2 uv = (Position.xy + offsets[i])* invDim;
		//float3 color = DrawMandelbrot(uv, shaderParams.Frame);
		color += DrawJulizSet(uv, shaderParams.Frame);
	}
	color /= SAMPLES;
	//float3 color = { 1.f,0.f,1.f };
	//return float4(shaderParams.Frame%70/70.f,0,0, 1.f);
	return float4(color, 1.f);
}