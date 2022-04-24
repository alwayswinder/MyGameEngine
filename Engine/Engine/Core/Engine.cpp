#if !defined(SHIPPING)

#include "../Content/ContentLoader.h"
#include "../Components/Script.h"
#include <thread>

bool engine_initalize()
{
	bool result{ primal::content::load_game() };
	return result;
}

void engine_update()
{
	primal::script::update(10.f);
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void engine_shutdown()
{
	primal::content::unload_game();
}

#endif