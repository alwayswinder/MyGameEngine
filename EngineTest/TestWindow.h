#pragma once
#include "Test.h"
#include "..\Engine\Platform\Platform.h"
#include "../Engine/Platform/PlatformTypes.h"

using namespace primal;
platform::window _windows[4];

LRESULT win_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	switch (msg)
	{
	case WM_DESTROY:
	{
		bool all_close{ true };
		for (u32 i{ 0 }; i < _countof(_windows); i++)
		{
			if (!_windows[i].is_closed())
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
class engine_test : public test 
{
public:
	bool initialize()override
	{
		platform::window_init_info info[]
		{
			{ &win_proc, nullptr, L"Test window 1", 100, 100, 400, 400},
			{ &win_proc, nullptr, L"Test window 2", 200, 200, 500, 500 },
			{ &win_proc, nullptr, L"Test window 3", 300, 300, 600, 600 },
			{ &win_proc, nullptr, L"Test window 4", 400, 400, 700, 700 },
		};
		static_assert(_countof(info) == _countof(_windows));

		for (u32 i{ 0 }; i<_countof(_windows);i++)
		{
			_windows[i] = platform::create_window(&info[i]);
		}
		return true;
	}
	void run()override
	{
		std::this_thread::sleep_for(std::chrono::milliseconds(10));
	}
	void shutdown()override
	{
		for (u32 i{0}; i<_countof(_windows); ++i)
		{
			platform::remove_window(_windows[i].get_id());
		}
	}
};