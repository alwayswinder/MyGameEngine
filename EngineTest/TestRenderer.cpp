#include "TestRenderer.h"
#include "..\Engine\Platform\Platform.h"
#include "..\Engine\Platform\PlatformTypes.h"
#include "..\Engine\Graphics\Renderer.h"

using namespace primal;

graphics::render_surface _surfaces[4];

LRESULT win_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	switch (msg)
	{
	case WM_DESTROY:
	{
		bool all_close{ true };
		for (u32 i{ 0 }; i < _countof(_surfaces); i++)
		{
			if (!_surfaces[i].window.is_closed())
			{
				all_close = false;
			}
		}
		if (all_close)
		{
			PostQuitMessage(0);
			return 0;
		}
	}
	break;
	case WM_SYSCHAR:
		if (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN))
		{
			platform::window win{ platform::window_id{(id::id_type)GetWindowLongPtr(hwnd, GWLP_USERDATA)} };
			win.set_fullscreen(!win.is_fullscreen());
			return 0;
		}
		break;
	}
	return DefWindowProc(hwnd, msg, wparam, lparam);
}

void create_render_surface(graphics::render_surface& surface, platform::window_init_info info)
{
	surface.window = platform::create_window(&info);
}

void destroy_render_surface(graphics::render_surface& surface)
{
	platform::remove_window(surface.window.get_id());
}


bool engine_test::initialize()
{
	//bool result{ graphics::initialize(graphics::graphics_platform::direct3d12) };
	//if (!result) return result;

	platform::window_init_info info[]
	{
		{ &win_proc, nullptr, L"Test window 1", 100, 100, 400, 400},
		{ &win_proc, nullptr, L"Test window 2", 200, 200, 500, 500 },
		{ &win_proc, nullptr, L"Test window 3", 300, 300, 600, 600 },
		{ &win_proc, nullptr, L"Test window 4", 400, 400, 700, 700 },
	};
	static_assert(_countof(info) == _countof(_surfaces));

	for (u32 i{ 0 }; i < _countof(_surfaces); i++)
	{
		create_render_surface(_surfaces[i], info[i]);
	}
	return true;
}
void engine_test::run()
{
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}
void engine_test::shutdown()
{
	for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
	{
		destroy_render_surface(_surfaces[i]);
	}

	//graphics::shutdown();
}