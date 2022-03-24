
#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif // !EDITOR_INTERFACE


#include "Engine/Common/CommonHeaders.h"
#include "Engine/Common/id.h"
#include "Engine/Components/Entity.h"
#include "Engine/Components/Transform.h"


using namespace primal;


EDITOR_INTERFACE
id::id_type CreateGameEntity()
{

}