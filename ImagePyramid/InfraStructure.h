#pragma once


//�ϼh,�t�d�޲z�h�i��
class __declspec(dllexport) CImageLayer {


};
 
class __declspec(dllexport) CImageInfo {
public:
	CImageInfo(int* position,unsigned char* imageBuffer);
	

private:
	int* m_ImagePosition; //�w�q���Ϫ���m Left,TOp,Width,Height

	unsigned char* m_ImageBuffer; //�x�s�v��Buffer

};

 