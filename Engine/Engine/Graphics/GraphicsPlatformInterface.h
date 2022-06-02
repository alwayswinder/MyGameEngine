#pragma once
#include "CommonHeaders.h"
#include "Renderer.h"

namespace primal::graphics
{
	struct platform_interface
	{
		bool(*initialize)();
		void(*shutdown)();
		void(*render)();
	};
}