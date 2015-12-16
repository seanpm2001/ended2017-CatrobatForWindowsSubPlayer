#pragma once

#include "Brick.h"
#include "FormulaTree.h"

namespace ProjectStructure
{
	class WaitBrick :
		public Brick
	{
	public:
		WaitBrick(FormulaTree *timeToWaitInSeconds, Script* parent);
		void Execute();
	private:
		FormulaTree *m_timeToWaitInSeconds;
	};
}