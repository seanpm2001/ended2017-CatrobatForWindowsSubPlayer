#pragma once
#include "brick.h"
class PlaceAtBrick :
	public Brick
{
public:
	PlaceAtBrick(string spriteReference, float positionX, float positionY, Script *parent);

private:
	float m_positionX;
	float m_positionY;
};
