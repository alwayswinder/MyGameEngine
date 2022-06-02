#pragma once

//#pragma  warning(disable: 4530)
//c++
#include <stdint.h>
#include <assert.h>
#include <typeinfo>
#include <memory>
#include <unordered_map>


//windows
#if defined(_WIN64)
#include <DirectXMath.h>
#endif

#ifndef DISABLE_COPY
#define DISABLE_COPY(T)			\
explicit T(const T&) = delete;	\
T& operator=(const T&) = delete;
#endif

#ifndef DISABLE_MOVE
#define DISABLE_MOVE(T)			\
explicit T(T&&) = delete;		\
T& operator=(T&&) = delete;
#endif

#ifndef DISABLE_COPY_AND_MOVE
#define DISABLE_COPY_AND_MOVE(T) DISABLE_COPY(T) DISABLE_MOVE(T)
#endif

//Common
#include "PrimitiveTypes.h"
#include "../Utilities/Math.h"
#include "../Utilities/utilities.h"
#include "../Utilities/MathTypes.h"
#include "id.h"

#ifdef _DEBUG
#define  DEBUG_OP(x) x
#else
#define DEBUG_OP(x) (void(0))
#endif // _DEBUG
