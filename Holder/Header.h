#pragma once
#include <windows.h>
#define HOLDER_API extern "C" __declspec(dllexport) 

extern HWND m_MainWindow;
extern HWND m_ContainerWin;
int CollectAllExplorerWindow();
void CreateContainerWindow(HWND handle);
void HoldWindow(HWND win, bool exist = 0);
void ReleaseWin(HWND win);
