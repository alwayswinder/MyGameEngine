#pragma once


#include "../../Common/commonHeaders.h"
#include "../Renderer.h"
#include "../../Platform/Window.h"

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <dxgi1_6.h>
#include <d3d12.h>
#include <wrl.h>


#pragma comment(lib, "dxgi.lib")
#pragma comment(lib, "d3d12.lib")

namespace primal::graphics::d3d12
{
	constexpr u32 frame_buffer_count{ 3 };
	using id3d12_device = ID3D12Device6;
	using id3d12_graphics_command_list = ID3D12GraphicsCommandList5;
}

#ifdef _DEBUG
#ifndef DXCall
#define DXCall(x)								   \
if (FAILED(x))									   \
{												   \
	char line_number[32];						   \
	sprintf_s(line_number, "%u", __LINE__);		   \
	OutputDebugStringA("Error in: ");			   \
	OutputDebugStringA(__FILE__);				   \
	OutputDebugStringA("\nLine: ");				   \
	OutputDebugStringA(line_number);			   \
	OutputDebugStringA("\n");					   \
	OutputDebugStringA(#x);						   \
	OutputDebugStringA("\n");					   \
	__debugbreak();								   \
}
#endif
#else
#ifndef DXCall
#define DXCall(x) x
#endif

#endif // _DEBUG


#ifdef _DEBUG
#define NAME_D3D12_OBJECT(obj, name) obj->SetName(name); OutputDebugString(L"::D3D12 object Created: ");\
	OutputDebugString(name); OutputDebugString(L"\n");
#define  NAME_D3D12_OBJECT_INDEXED(obj, n, name)		  \
{														  \
	wchar_t full_name[128];								  \
	if (swprintf_s(full_name, L"%s[%u]", name, n) > 0)	  \
	{													  \
		obj->SetName(full_name);						  \
		OutputDebugString(L"::D3D12 object Created: ");	  \
		OutputDebugString(full_name);					  \
		OutputDebugString(L"\n");						  \
	}													  \
}
#else
#define NAME_D3D12_OBJECT(x, name)
#define NAME_D3D12_OBJECT_INDEXED(x, n, name)

#endif // _DEBUG


#include "D3D12Helpers.h"
#include "D3D12Resource.h"