#include "Header.h"

HWND m_MainWindow;
HWND m_ContainerWin;
HWINEVENTHOOK m_Hook;
void (*m_WindowEvent)(bool, HANDLE) = nullptr;

VOID CALLBACK WinEventProc(
    HWINEVENTHOOK hWinEventHook,
    DWORD event,
    HWND hwnd,
    LONG idObject,
    LONG idChild,
    DWORD dwEventThread,
    DWORD dwmsEventTime
)
{
    if (event == EVENT_OBJECT_CREATE && idChild == CHILDID_SELF)
    {
        // Get window class name
        char className[256];
        GetClassNameA(hwnd, className, sizeof(className));

        // Compare with explorer class name
        if (strcmp(className, "CabinetWClass") == 0) {
            //std::cout << "Explorer window " << hwnd << " created." << std::endl;
            HoldWindow(hwnd);
            m_WindowEvent(true, hwnd);
        }
    }
    // else if (event == EVENT_OBJECT_DESTROY && idChild == CHILDID_SELF)
    // {
    //     char className[256] = { 0 };
    //     GetClassNameA(hwnd, className, sizeof(className));
    //     if (strcmp(className, "CabinetWClass") == 0) {
    //        // std::cout << "Explorer window " << hwnd << " destroyed." << std::endl;
    //         m_WindowEvent(false, hwnd);
    //         ReleaseWin(hwnd);
    //     }
    // }
}

HOLDER_API int SetMainWindow(HWND handle, void (*callback)(bool, HANDLE))
{
    m_WindowEvent = callback;
    m_MainWindow = handle;
    //SetWindowLong(handle, GWL_STYLE, WS_VISIBLE | WS_CLIPCHILDREN | WS_OVERLAPPEDWINDOW);
    CreateContainerWindow(handle);
    int count = CollectAllExplorerWindow(); 
    m_Hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE/*EVENT_OBJECT_DESTROY*/, NULL, WinEventProc,0,0,WINEVENT_OUTOFCONTEXT);
    return count;
}

HOLDER_API void ReleaseMainWindow()
{
    UnhookWinEvent(m_Hook);
}

HOLDER_API void CloseExplorerWin(HWND wnd)
{
    ReleaseWin(wnd);
    SendMessage(wnd, WM_CLOSE, 0, 0);
    m_WindowEvent(false, wnd);
}