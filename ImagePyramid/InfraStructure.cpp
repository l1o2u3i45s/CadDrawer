#include "InfraStructure.h"

 
CImageInfo::CImageInfo(int* position, unsigned char* imageBuffer)
{
	m_ImagePosition = position;
	m_ImageBuffer = imageBuffer;
}

int* CImageInfo::GetImageInfo()
{
	return m_ImagePosition;
}

unsigned char* CImageInfo::GetImageBuffer()
{
	return m_ImageBuffer;
}
