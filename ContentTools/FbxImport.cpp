#include "FbxImport.h"
#include "Geometry.h"

#if _DEBUG
#pragma comment(lib, "C:\\Program Files\\Autodesk\\FBX\\FBX SDK\\2020.2.1\\lib\\vs2019\\x64\\debug\\libfbxsdk-md.lib")
#pragma comment(lib, "C:\\Program Files\\Autodesk\\FBX\\FBX SDK\\2020.2.1\\lib\\vs2019\\x64\\debug\\libxml2-md.lib")
#pragma comment(lib, "C:\\Program Files\\Autodesk\\FBX\\FBX SDK\\2020.2.1\\lib\\vs2019\\x64\\debug\\zlib-md.lib")

#else

#endif

namespace primal::tools
{
	namespace
	{
		std::mutex fbx_mutex;
	}
	bool fbx_context::initialize_fbx()
	{
		assert(!is_valid());
		_fbx_manager = FbxManager::Create();
		if (!_fbx_manager)
		{
			return false;
		}
		FbxIOSettings* ios{ FbxIOSettings::Create(_fbx_manager, IOSROOT) };
		assert(ios);
		_fbx_manager->SetIOSettings(ios);

		return true;
	}
	void fbx_context::load_fbx_file(const char* file)
	{
		assert(_fbx_manager && !_fbx_scene);
		_fbx_scene = FbxScene::Create(_fbx_manager, "Importer Scene");
		if (!_fbx_scene)
		{
			return;
		}
		FbxImporter* importer{ FbxImporter::Create(_fbx_manager, "Importer") };
		if (!(importer && importer->Initialize(file, -1, _fbx_manager->GetIOSettings()) && importer->Import(_fbx_scene)))
		{
			return;
		}
		importer->Destroy();

		_scene_scale = (f32)_fbx_scene->GetGlobalSettings().GetSystemUnit().GetConversionFactorTo(FbxSystemUnit::m);
	}
	void fbx_context::get_scene(FbxNode* root /* = nullptr */)
	{
		assert(is_valid());
		if (!root)
		{
			root = _fbx_scene->GetRootNode();
			if (!root)return;
		}

		const s32 num_nodes{ root->GetChildCount() };
		for (s32 i = 0; i < num_nodes; i++)
		{
			FbxNode* node{ root->GetChild(i) };
			if(!node)continue;
			if (node->GetMesh())
			{
				lod_group lod{};
				get_mesh(node, lod.meshes);
				if (lod.meshes.size())
				{
					lod.name = lod.meshes[0].name;
					_scene->lod_groups.emplace_back(lod);
				}
			}
			else if (node->GetLodGroup())
			{
				get_lod_group(node);
			}
			else
			{
				get_scene(node);
			}
		}
	}
	void fbx_context::get_mesh(FbxNode* node, utl::vector<mesh>& meshes)
	{
		assert(node);
		if (FbxMesh * fbx_mesh{ node->GetMesh() })
		{
			if (fbx_mesh->RemoveBadPolygons() < 0) return;

			FbxGeometryConverter gc{ _fbx_manager };
			fbx_mesh = static_cast<FbxMesh*>(gc.Triangulate(fbx_mesh, true));
			if (!fbx_mesh || fbx_mesh->RemoveBadPolygons() < 0) return;

			mesh m;
			m.lod_id = (u32)meshes.size();
			m.lod_threshold = -1.f;
			m.name = (node->GetName()[0] != '\0') ? node->GetName() : fbx_mesh->GetName();

			if (get_mesh_data(fbx_mesh, m))
			{
				meshes.emplace_back(m);
			}
		}
	}
	void fbx_context::get_lod_group(FbxNode* node)
	{

	}

	bool fbx_context::get_mesh_data(FbxMesh * fbx_mesh, mesh& m)
	{
		assert(fbx_mesh);

		const s32 num_polys{ fbx_mesh->GetPolygonCount() };
		if (num_polys <= 0)return false;

		const s32 num_vertices{ fbx_mesh->GetControlPointsCount() };
		FbxVector4* vertices{ fbx_mesh->GetControlPoints() };
		const s32 num_indices{ fbx_mesh->GetPolygonVertexCount() };
		s32* indices{ fbx_mesh->GetPolygonVertices() };

		assert(num_vertices > 0 && vertices && num_indices > 0 && indices);
		if (!(num_vertices > 0 && vertices && num_indices > 0 && indices))return false;

		m.raw_indices.resize(num_indices);
		utl::vector vertex_ref(num_vertices, u32_invalid_id);

		for (s32 i{ 0 }; i<num_indices; ++i)
		{
			const s32 v_idx{ indices[i] };
			if (vertex_ref[v_idx] != u32_invalid_id)
			{
				m.raw_indices[i] = vertex_ref[v_idx];
			}
			else
			{
				FbxVector4 v = vertices[v_idx] * _scene_scale;
				m.raw_indices[i] = (u32)m.positions.size();
				vertex_ref[v_idx] = m.raw_indices[i];
				m.positions.emplace_back((f32)v[0], (f32)v[1], (f32)v[2]);
			}
		}
		assert(m.raw_indices.size() % 3 == 0);

		assert(num_polys > 0);
		FbxLayerElementArrayTemplate<s32>* mtl_indices;
		if (fbx_mesh->GetMaterialIndices(&mtl_indices))
		{
			for (s32 i{0}; i<num_polys; ++i)
			{
				const s32 mtl_index{ mtl_indices->GetAt(i) };
				assert(mtl_indices >= 0);
				m.material_indices.emplace_back((u32)mtl_index);
				if (std::find(m.material_used.begin(), m.material_used.end(), (u32)mtl_index) == m.material_used.end())
				{
					m.material_used.emplace_back((u32)mtl_index);
				}
			}
		}
		const bool import_normals{ !_scene_data->settings.calculate_normals };
		const bool import_tangents{ !_scene_data->settings.calculate_tangents };

		if (import_normals)
		{
			FbxArray<FbxVector4> normals;
			if()
		}
	}

	EDITOR_INTERFACE void ImportFbx(const char* file, scene_data* data)
	{
		assert(file && data);
		scene scene{};

		{
			std::lock_guard lock{ fbx_mutex };
			fbx_context fbx_contex{ file, &scene, data };
			if (fbx_contex.is_valid())
			{

			}
			else
			{
				return;
			}
		}
		process_scene(scene, data->settings);
		pack_data(scene, *data);
	}
}