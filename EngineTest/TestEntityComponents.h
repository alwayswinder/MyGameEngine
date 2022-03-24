#pragma once
#include "Test.h"


using namespace primal;


class engine_test : public test
{
public:
	bool initialize()override { return true; }
	void run()override {}
	void shutdown()override {}
};