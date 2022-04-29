#include "PrimitiveMesh.h"
#include "Geometry.h"

namespace primal::tools
{
	namespace
	{

	}

	EDITOR_INTERFACE void CreatePrimitiveMesh(scene_data* data, primitive_init_info* info)
	{
		assert(data && info);
		assert(info->type < primitive_mesh_type::count);
		
	}
}