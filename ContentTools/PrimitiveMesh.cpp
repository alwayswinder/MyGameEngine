#include "PrimitiveMesh.h"
#include "Geometry.h"

namespace primal::tools
{
	namespace
	{
		using namespace math;
		using primitive_mesh_creator = void(*)(scene&, const primitive_init_info& info);

		void create_plane(scene& scene, const primitive_init_info& info);
		void create_cube(scene& scene, const primitive_init_info& info);
		void create_uv_sphere(scene& scene, const primitive_init_info& info);
		void create_ico_sphere(scene& scene, const primitive_init_info& info);
		void create_cylinder(scene& scene, const primitive_init_info& info);
		void create_capsule(scene& scene, const primitive_init_info& info);

		primitive_mesh_creator creators[]
		{
			create_plane,
			create_cube,
			create_uv_sphere,
			create_ico_sphere,
			create_cylinder,
			create_capsule,
		};
		static_assert(_countof(creators) == primitive_mesh_type::count);

		mesh create_plane(const primitive_init_info& info,
			u32 horizontal_index = 0, u32 vertical_index = 2, bool flip_winding = false,
			v3 offset = { -0.5f, 0.f, -0.5f }, v2 u_range = { 0.f,1.f }, v2 v_range = { 0.f, 1.f })
		{

		}
		void create_plane(scene& scene, const primitive_init_info& info)
		{
			lod_group lod{};
			lod.name = "plane";
			lod.meshes.emplace_back(create_plane(info));
			scene.lod_groups.emplace_back(lod);
		}

		void create_cube(scene& scene, const primitive_init_info& info) 
		{

		}
			
		void create_uv_sphere(scene& scene, const primitive_init_info& info)
		{

		}

		void create_ico_sphere(scene& scene, const primitive_init_info& info)
		{

		}

		void create_cylinder(scene& scene, const primitive_init_info& info)
		{

		}

		void create_capsule(scene& scene, const primitive_init_info& info)
		{

		}
	}

	EDITOR_INTERFACE void CreatePrimitiveMesh(scene_data* data, primitive_init_info* info)
	{
		assert(data && info);
		assert(info->type < primitive_mesh_type::count);
		scene scene{};
		creators[info->type](scene, *info);
	}
}