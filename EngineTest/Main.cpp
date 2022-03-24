
#include "Test.h"

#define  TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponents.h"


#else
#error One of the test need to be enabled
#endif


int main()
{
	engine_test test{};
	if (test.initialize())
	{
		test.run();
	}


	test.shutdown();
}