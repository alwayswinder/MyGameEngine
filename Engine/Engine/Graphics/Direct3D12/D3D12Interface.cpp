#include "D3D12Interface.h"
#include "CommonHeaders.h"
#include "../GraphicsPlatformInterface.h"


namespace primal::graphics::d3d12
{
	void get_platform_interface(platform_interface& pi)
	{
		pi.initialize();
	}
}