#pragma once

#include "ComponentsCommon.h"
#include <iostream>

namespace primal
{
#define  INIT_INFO(component) namespace component {struct Init_info;}
	INIT_INFO(transform);
	INIT_INFO(script);

#undef INIT_INFO
	namespace transform{struct Init_info;}
	namespace game_entity
	{
		struct entity_info
		{
			transform::Init_info* transform{ nullptr };
			script::Init_info* script{ nullptr };
		};
		entity create(entity_info info);
		void remove(entity_id id);
		bool is_alive(entity_id id);
	}
}

