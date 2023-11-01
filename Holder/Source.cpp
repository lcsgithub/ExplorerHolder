#include <vector>
#include "Header.h"

std::vector<HWND> m_ExplorerWindows;
HWND m_ActiveExplorerWin = 0;
#define YOFFSET 31
#define PADDING 4
void HoldWindow(HWND win, bool exist)
{
	if (!exist)
		m_ExplorerWindows.push_back(win);
	HWND hWnd = m_ContainerWin;
	RECT rc;
	GetClientRect(hWnd, &rc);
	SetForegroundWindow(win);
    SetWindowLong(win, GWL_STYLE, WS_CHILD | WS_VISIBLE);
	SetParent(win, hWnd);
	SetWindowPos(win, 0, 0, -YOFFSET, rc.right - rc.left, rc.bottom - rc.top+YOFFSET, SWP_HIDEWINDOW);
}
HOLDER_API void ShowExplorerWin(HWND win)
{
	if (m_ActiveExplorerWin != 0)
		ShowWindow(m_ActiveExplorerWin, SW_HIDE);
    HWND hWnd = m_ContainerWin;
    RECT rc;
    GetWindowRect(hWnd, &rc);
	SetForegroundWindow(win);
    SetWindowPos(win, 0, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
	m_ActiveExplorerWin = win;
}

int CollectAllExplorerWindow()
{
    EnumWindows([](HWND hwnd, LPARAM lparam)->BOOL
        {
            char className[256];
            GetClassNameA(hwnd, className, sizeof(className));

            // Compare with explorer class name
            if (strcmp(className, "CabinetWClass") == 0) {
                m_ExplorerWindows.push_back(hwnd);
            }
            return TRUE;
        }, 0);
    for (auto& win : m_ExplorerWindows)
        HoldWindow(win, true);
    if (m_ExplorerWindows.size() > 0)
        ShowExplorerWin(m_ExplorerWindows[0]);
	return m_ExplorerWindows.size();
}
void ReleaseWin(HWND wnd)
{
	for (auto b = m_ExplorerWindows.begin(); b != m_ExplorerWindows.end(); ++b)
	{
		if (*b == wnd)
		{
			m_ExplorerWindows.erase(b);
			break;
		}
	}
}

HOLDER_API HWND GetExplorerWindow(int index)
{
    return index < m_ExplorerWindows.size() ? m_ExplorerWindows[index] : nullptr;
}
HOLDER_API int GetExplorerWindowCount()
{
	return m_ExplorerWindows.size();
}

HOLDER_API void UpdateMainWinSize()
{
	RECT rc;
	GetClientRect(m_MainWindow, &rc);
	int w = rc.right - rc.left;
	int h = rc.bottom - rc.top;
	w -= PADDING * 2;
	h -= PADDING * 2 + YOFFSET;
	SetWindowPos(m_ContainerWin, 0, 0, 0, w, h, SWP_NOMOVE | SWP_NOREPOSITION);
	for (auto& win : m_ExplorerWindows)
	{
		SetWindowPos(win, 0, 0, 0, w, h + YOFFSET, SWP_NOMOVE | SWP_NOREPOSITION);
	}
}

LRESULT CALLBACK ContainerWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	//switch (message)
	//{
	//case WM_LBUTTONDOWN:
	//case WM_LBUTTONUP:
	//case WM_MOUSEMOVE:
	//{
	//	int x = LOWORD(lParam);
	//	int y = HIWORD(lParam);
	//	if(m_ActiveExplorerWin != 0)
	//		y -= 32;
	//	lParam = MAKEWORD(x, y);
	//}
	//	break;
	//}
	return m_ActiveExplorerWin != 0 ? SendMessage(m_ActiveExplorerWin, message, wParam, lParam)
		: DefWindowProc(hWnd, message, wParam, lParam);
}

void CreateContainerWindow(HWND handle)
{
	WNDCLASSEXW wcex;
	HINSTANCE hInstance = (HINSTANCE)GetWindowLongPtr(handle, GWLP_HINSTANCE);
	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = ContainerWndProc;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = 0;
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wcex.lpszMenuName = 0;
	wcex.lpszClassName = L"ClassM";
	wcex.hIconSm = 0;

	RECT rc;
	GetClientRect(handle, &rc);
	RegisterClassExW(&wcex);
	HWND hWnd = CreateWindowW(L"ClassM", L"ExplorerHolder", WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_CHILDWINDOW,
		PADDING, YOFFSET+ PADDING, rc.right-rc.left- PADDING *2, rc.bottom-rc.top- PADDING *2 - YOFFSET, handle, nullptr, hInstance, nullptr);
	//CreateWindowA("BUTTON", "Title", WS_VISIBLE | WS_CHILD, 2, 0, 100, 25, hWnd, (HMENU)(0x330), hInstance, 0);

	ShowWindow(hWnd, SW_SHOW);
	m_ContainerWin = hWnd;
}
extern "C" __declspec(dllexport)  HWND GetContainer()
{
	return m_ContainerWin;
}