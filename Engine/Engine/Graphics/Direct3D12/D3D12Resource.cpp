#include "D3D12Resource.h"


namespace primal::graphics::d3d12
{
	///descriptor_heap
	bool descriptor_heap::initialize(u32 capacity, bool is_shader_visiable)
	{
		std::lock_guard lock{ _mutex };
	}
	void descriptor_heap::release()
	{

	}
	descriptor_handle descriptor_heap::allocate()
	{

	}
	void descriptor_heap::free(descriptor_handle& handle)
	{
		std::lock_guard lock{ _mutex };

	}
}