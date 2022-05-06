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
