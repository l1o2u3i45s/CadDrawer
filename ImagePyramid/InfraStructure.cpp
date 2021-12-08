#include "InfraStructure.h"

 
CImageInfo::CImageInfo(int* position, unsigned char* imageBuffer)
{
	m_ImagePosition = position;
	m_ImageBuffer = imageBuffer;
}
