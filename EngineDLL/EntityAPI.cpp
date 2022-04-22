
#include "Common.h"
#include "Engine/Common/CommonHeaders.h"
#include "Engine/Common/id.h"
#include "Engine/Components/Entity.h"
#include "Engine/Components/Transform.h"
#include "Engine/Components/Script.h"

using namespace primal;

namespace
{
	struct transform_component
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];
		transform::Init_info to_init_info()
		{
			using namespace DirectX;
			transform::Init_info info{};
			memcpy(&info.position[0], &position[0], sizeof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(scale));
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);
			memcpy(&info.rotation[0], &rot_quat.x, sizeof(rotation));
			return info;
		}
	};
	struct script_component
	{
		script::detail::script_creator script_creator;
		script::Init_info to_init_info()
		{
			script::Init_info info{};
			info.script_creator = script_creator;
			return info;
		}
	};
	struct game_entity_descriptor
	{
		transform_component transfrom;
		script_component script;
	};
	game_entity::entity entity_from_id(id::id_type id)
	{
		return game_entity::entity{ game_entity::entity_id{id} };
	}
}

EDITOR_INTERFACE
id::id_type CreateGameEntity(game_entity_descriptor* e)
{
	assert(e);
	game_entity_descriptor& desc{ *e };
	transform::Init_info transform_info{ desc.transfrom.to_init_info() };
	script::Init_info script_info{ desc.script.to_init_info() };

	game_entity::entity_info entity_info
	{
		&transform_info, 
		&script_info,
	};

	return game_entity::create(entity_info).get_id();
}

EDITOR_INTERFACE
void RemoveGameEntity(id::id_type id)
{
	assert(id::is_valid(id));
	game_entity::remove(game_entity::entity_id{ id });
}