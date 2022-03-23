#pragma once
#include "ComponentsCommon.h"

namespace primal::transform
{
	struct Init_info 
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{ 1.f,1.f,1.f };
	};
	component create_transfrom(const Init_info& info, game_entity::entity e);
	void remove_transform(component c);
}