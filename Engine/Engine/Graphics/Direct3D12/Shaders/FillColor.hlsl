#include "Fractals.hlsli"

struct ShaderConstancs
{
	float Width;
	float Height;
	uint  Frame;
};

ConstantBuffer<ShaderConstancs>			shaderParams	: register(b1);

float4 FillColorPS(in noperspective float4 Position : SV_Position,
				   in noperspective float2 UV : TEXCOORD) : SV_Target0
{
	const float2 invDim = float2(1.f/shaderParams.Width, 1.f/shaderParams.Height);
	const float2 uv = (Position.xy) * invDim;  
	float3 color = DrawMandelbrot(uv);	
	//float3 color = { 1.f,0.f,1.f };
	return float4(color, 1.f);
}