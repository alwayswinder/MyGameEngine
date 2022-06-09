#pragma once
#include "D3D12CommonHeaders.h"

namespace primal::graphics::d3d12::d3dx
{
	constexpr struct
	{
		const D3D12_HEAP_PROPERTIES default_heap
		{
			D3D12_HEAP_TYPE_DEFAULT,
			D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
			D3D12_MEMORY_POOL_UNKNOWN,
			0,
			0
		};
	}heap_properties;

	ID3D12RootSignature* create_root_signature(const D3D12_ROOT_SIGNATURE_DESC1& desc);

	struct d3d12_descriptor_range : public D3D12_DESCRIPTOR_RANGE1
	{
		constexpr explicit d3d12_descriptor_range(D3D12_DESCRIPTOR_RANGE_TYPE range_type,
			u32 descriptor_count, u32 shader_register, u32 space = 0,
			D3D12_DESCRIPTOR_RANGE_FLAGS Flags = D3D12_DESCRIPTOR_RANGE_FLAG_NONE,
			u32 offset_from_table_start = D3D12_DESCRIPTOR_RANGE_OFFSET_APPEND)
			:D3D12_DESCRIPTOR_RANGE1{ range_type, descriptor_count, shader_register, space, Flags, offset_from_table_start }
		{}
	};
	struct d3d12_root_parameter : public D3D12_ROOT_PARAMETER1
	{
		constexpr void as_constants(u32 num_constants, D3D12_SHADER_VISIBILITY visibility,
			u32 shader_register, u32 space = 0)
		{
			ParameterType = D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS;
			ShaderVisibility = visibility;
			Constants.Num32BitValues = num_constants;
			Constants.ShaderRegister = shader_register;
			Constants.RegisterSpace = space;
		}
		constexpr void as_cbv(D3D12_SHADER_VISIBILITY visibility,
			u32 shader_register, u32 space = 0,
			D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAG_NONE)
		{
			as_descriptor(D3D12_ROOT_PARAMETER_TYPE_CBV, visibility, shader_register, space, flags);
		}
		constexpr void as_srv(D3D12_SHADER_VISIBILITY visibility,
			u32 shader_register, u32 space = 0,
			D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAG_NONE)
		{
			as_descriptor(D3D12_ROOT_PARAMETER_TYPE_SRV, visibility, shader_register, space, flags);
		}
		constexpr void as_uav(D3D12_SHADER_VISIBILITY visibility,
			u32 shader_register, u32 space = 0,
			D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAG_NONE)
		{
			as_descriptor(D3D12_ROOT_PARAMETER_TYPE_UAV, visibility, shader_register, space, flags);
		}

		constexpr void as_descriptor_table(D3D12_SHADER_VISIBILITY visibility,
			const d3d12_descriptor_range* ranges, u32 range_count)
		{
			ParameterType = D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE;
			ShaderVisibility = visibility;
			DescriptorTable.NumDescriptorRanges = range_count;
			DescriptorTable.pDescriptorRanges = ranges;
		}
	private:
		constexpr void as_descriptor(D3D12_ROOT_PARAMETER_TYPE type, D3D12_SHADER_VISIBILITY visibility,
			u32 shader_register, u32 space, D3D12_ROOT_DESCRIPTOR_FLAGS flags)
		{
			ParameterType = type;
			ShaderVisibility = visibility;
			Descriptor.ShaderRegister = shader_register;
			Descriptor.RegisterSpace = space;
			Descriptor.Flags = flags;
		}
	};

	struct d3d12_root_signature_desc :public D3D12_ROOT_SIGNATURE_DESC1
	{
		constexpr explicit d3d12_root_signature_desc(const d3d12_root_parameter* parameters,
			u32 parameter_count, const D3D12_STATIC_SAMPLER_DESC* static_samplers = nullptr,
			u32 sampler_count = 0, D3D12_ROOT_SIGNATURE_FLAGS flags =
			D3D12_ROOT_SIGNATURE_FLAG_DENY_HULL_SHADER_ROOT_ACCESS |
			D3D12_ROOT_SIGNATURE_FLAG_DENY_DOMAIN_SHADER_ROOT_ACCESS |
			D3D12_ROOT_SIGNATURE_FLAG_DENY_GEOMETRY_SHADER_ROOT_ACCESS)
			:D3D12_ROOT_SIGNATURE_DESC1{ parameter_count, parameters, sampler_count, static_samplers, flags}
		{}
		ID3D12RootSignature* create()const
		{
			return create_root_signature(*this);
		}
	};
}