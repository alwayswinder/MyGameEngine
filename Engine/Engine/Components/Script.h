#pragma once
#include "ComponentsCommon.h"

namespace primal::script
{
	struct Init_info
	{
		detail::script_creator script_creator;
	};
	component create(Init_info info, game_entity::entity e);
	void remove(component c);
	void update(float dt);
}