#pragma once

#include "ComponentsCommon.h"
#include <iostream>

namespace primal
{
#define  INIT_INFO(component) namespace component {struct Init_info;}
	INIT_INFO(transform);
#undef INIT_INFO
	namespace transform{struct Init_info;}
	namespace game_entity
	{
		struct entity_info
		{
			transform::Init_info* transform{ nullptr };
		};
		entity create_game_entity(const entity_info& info);
		void remove_game_entity(entity e);
		bool is_alive(entity e);
	}
}

