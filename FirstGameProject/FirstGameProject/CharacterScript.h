#pragma once
#include <string>
#include "GameEntity.h"

namespace first_game_project
{
	REGIESTER_SCRIPT(character_script);
	class character_script : public primal::script::entity_script
	{
	public:
		constexpr explicit character_script(primal::game_entity::entity entity)
			:primal::script::entity_script(entity)
		{}
		void update(float dt)override;
	};
}

