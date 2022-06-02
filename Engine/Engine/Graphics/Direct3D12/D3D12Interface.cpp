#include "D3D12Interface.h"
#include "CommonHeaders.h"
#include "../GraphicsPlatformInterface.h"
#include "D3D12Core.h"

namespace primal::graphics::d3d12
{
	void get_platform_interface(platform_interface& pi)
	{
		pi.initialize = core::initialize;
		pi.shutdown = core::shutdown;
		pi.render = core::render;
	}
}