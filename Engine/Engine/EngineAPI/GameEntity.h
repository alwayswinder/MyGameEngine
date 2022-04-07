#pragma once


#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"

namespace primal
{
	namespace game_entity
	{
		DEFINE_TYPED_ID(entity_id);

		class entity
		{
		public:
			constexpr explicit entity(entity_id id) :_id{ id } {}
			constexpr entity() : _id{ id::invalid_id } {}
			constexpr entity_id get_id()const { return _id; }
			constexpr bool is_valid()const { return id::is_valid(_id); }

			transform::component transform()const;
		private:
			entity_id _id;
		};
	}
	namespace script
	{
		class entity_script : public game_entity::entity
		{
		public:
			virtual ~entity_script() = default;
			virtual void begin_play(){}
			virtual void update(float){}
		protected:
			constexpr explicit entity_script(game_entity::entity entity)
				:game_entity::entity{entity}{}
		};

		class my_player_character :public entity_script
		{
		public:
			void update(float dt)override
			{
				//
			}
		};
		using script_ptr = std::unique_ptr<entity_script>;
		using script_creator = script_ptr(*)(game_entity::entity entity);

		template<class script_class>
		script_ptr create_script(game_entity::entity entity)
		{
			assert(entity.is_valid());
			
			return std::make_unique<script_ptr>(entity);
		}

	}
}