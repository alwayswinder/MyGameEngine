#define M_RE_START			-2.8f
#define M_RE_END			1.f
#define M_IM_START			-1.5f
#define M_IM_END			1.5f
#define M_MAX_ITERATION		1000

#define J_RE_START			-2.f
#define J_RE_END			2.f
#define J_IM_START			-1.5f
#define J_IM_END			1.5f
#define J_MAX_ITERATION		1000

float3 MapColor(float t)
{
	float3 ambient = float3(0.09f, 0.12f, 0.16f);
	
	return float3(3.f * t, 5.f * t, 10.f * t) + ambient;
}
float2 ComplexSq(float2 c)
{
	return float2(c.x * c.x - c.y * c.y, 2.f * c.x * c.y);
}

float3 DrawMandelbrot(float2 uv, uint frame)
{
	const float2 c = float2(M_RE_START + uv.x * (M_RE_END - M_RE_START),
							M_IM_START + uv.y * (M_IM_END - M_IM_START));
	float2 z = 0.f;
	if (frame > M_MAX_ITERATION)
	{
		frame = M_MAX_ITERATION;
	}
	for (int i = 0; i < M_MAX_ITERATION; i++)
	{
		z = ComplexSq(z) + c;
		const float d = dot(z, z);
		if (d > 4.f)
		{
			const float t = i;
			return MapColor(t / /*M_MAX_ITERATION*/frame);
		}
	}
	return 1.f;
}

float3 DrawJulizSet(float2 uv, uint frame)
{
	float2 z = float2(J_RE_START + uv.x * (J_RE_END - J_RE_START),
					  J_IM_START + uv.y * (J_IM_END - J_IM_START));
	const float f = frame * 0.002f;
	const float2 w = float2(cos(f), sin(f));
	const float2 c = (2.f * w - ComplexSq(w)) * 0.26f;

	for (int i = 0; i < J_MAX_ITERATION; i++)
	{
		z = ComplexSq(z) + c;
		const float d = dot(z, z);
		if (d > 4.f)
		{
			const float t = i + 1 - log(log2(d));
			return MapColor(t / J_MAX_ITERATION);
		}
 	}
	return 1.f;
}