#pragma once


//圖層,負責管理多張圖
class __declspec(dllexport) CImageLayer {


};
 
class __declspec(dllexport) CImageInfo {
public:
	CImageInfo(int* position,unsigned char* imageBuffer);
	

private:
	int* m_ImagePosition; //定義此圖的位置 Left,TOp,Width,Height

	unsigned char* m_ImageBuffer; //儲存影像Buffer

};

 