#include <windows.h>
#include <commctrl.h>
#include <vector>
#include <string>

#pragma comment(lib, "comctl32.lib")

struct Shape {
    int type; // 0=Line, 1=Rectangle, 2=Circle
    POINT start, end;
};

std::vector<Shape> shapes;
Shape currentShape;
bool isDrawing = false;
int currentTool = 0;
HWND hwndCanvas;

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK CanvasProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR, int nCmdShow) {
    WNDCLASSEX wc = {sizeof(WNDCLASSEX)};
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"BluePrintOne";
    wc.hbrBackground = CreateSolidBrush(RGB(245, 245, 245));
    wc.hCursor = LoadCursor(NULL, IDC_ARROW);
    RegisterClassEx(&wc);

    HWND hwnd = CreateWindowEx(0, L"BluePrintOne", L"BluePrintOne - Planning & Design",
        WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, 900, 600,
        NULL, NULL, hInstance, NULL);

    ShowWindow(hwnd, nCmdShow);
    UpdateWindow(hwnd);

    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    return msg.wParam;
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    static HWND hwndLine, hwndRect, hwndCircle, hwndClear, hwndSave;
    
    switch (msg) {
    case WM_CREATE: {
        HMENU hMenu = CreateMenu();
        HMENU hFileMenu = CreatePopupMenu();
        AppendMenu(hFileMenu, MF_STRING, 1, L"New");
        AppendMenu(hFileMenu, MF_STRING, 2, L"Save");
        AppendMenu(hFileMenu, MF_STRING, 3, L"Save As");
        AppendMenu(hFileMenu, MF_SEPARATOR, 0, NULL);
        AppendMenu(hFileMenu, MF_STRING, 4, L"Exit");
        AppendMenu(hMenu, MF_POPUP, (UINT_PTR)hFileMenu, L"File");
        
        HMENU hEditMenu = CreatePopupMenu();
        AppendMenu(hEditMenu, MF_STRING, 5, L"Undo");
        AppendMenu(hEditMenu, MF_STRING, 6, L"Clear");
        AppendMenu(hMenu, MF_POPUP, (UINT_PTR)hEditMenu, L"Edit");
        SetMenu(hwnd, hMenu);

        hwndLine = CreateWindow(L"BUTTON", L"Line", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
            10, 40, 80, 30, hwnd, (HMENU)10, NULL, NULL);
        hwndRect = CreateWindow(L"BUTTON", L"Rectangle", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
            100, 40, 100, 30, hwnd, (HMENU)11, NULL, NULL);
        hwndCircle = CreateWindow(L"BUTTON", L"Circle", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
            210, 40, 80, 30, hwnd, (HMENU)12, NULL, NULL);

        WNDCLASS canvasClass = {0};
        canvasClass.lpfnWndProc = CanvasProc;
        canvasClass.hInstance = GetModuleHandle(NULL);
        canvasClass.lpszClassName = L"CanvasClass";
        canvasClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
        canvasClass.hCursor = LoadCursor(NULL, IDC_CROSS);
        RegisterClass(&canvasClass);

        hwndCanvas = CreateWindow(L"CanvasClass", NULL, WS_CHILD | WS_VISIBLE | WS_BORDER,
            10, 80, 860, 460, hwnd, NULL, NULL, NULL);
        break;
    }
    case WM_COMMAND:
        switch (LOWORD(wParam)) {
        case 1: shapes.clear(); InvalidateRect(hwndCanvas, NULL, TRUE); break;
        case 2: MessageBox(hwnd, L"Save feature", L"BluePrintOne", MB_OK); break;
        case 3: {
            OPENFILENAME ofn = {sizeof(OPENFILENAME)};
            wchar_t file[260] = L"";
            ofn.hwndOwner = hwnd;
            ofn.lpstrFile = file;
            ofn.nMaxFile = 260;
            ofn.lpstrFilter = L"PNG\0*.png\0";
            ofn.Flags = OFN_OVERWRITEPROMPT;
            if (GetSaveFileName(&ofn)) {
                MessageBox(hwnd, L"Saved!", L"BluePrintOne", MB_OK);
            }
            break;
        }
        case 4: PostQuitMessage(0); break;
        case 5: if (!shapes.empty()) { shapes.pop_back(); InvalidateRect(hwndCanvas, NULL, TRUE); } break;
        case 6: shapes.clear(); InvalidateRect(hwndCanvas, NULL, TRUE); break;
        case 10: currentTool = 0; break;
        case 11: currentTool = 1; break;
        case 12: currentTool = 2; break;
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }
    return 0;
}

LRESULT CALLBACK CanvasProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg) {
    case WM_LBUTTONDOWN:
        isDrawing = true;
        currentShape.type = currentTool;
        currentShape.start.x = LOWORD(lParam);
        currentShape.start.y = HIWORD(lParam);
        currentShape.end = currentShape.start;
        break;
    case WM_MOUSEMOVE:
        if (isDrawing) {
            currentShape.end.x = LOWORD(lParam);
            currentShape.end.y = HIWORD(lParam);
            InvalidateRect(hwnd, NULL, FALSE);
        }
        break;
    case WM_LBUTTONUP:
        if (isDrawing) {
            currentShape.end.x = LOWORD(lParam);
            currentShape.end.y = HIWORD(lParam);
            shapes.push_back(currentShape);
            isDrawing = false;
            InvalidateRect(hwnd, NULL, TRUE);
        }
        break;
    case WM_PAINT: {
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hwnd, &ps);
        HPEN pen = CreatePen(PS_SOLID, 3, RGB(107, 70, 193));
        SelectObject(hdc, pen);
        SelectObject(hdc, GetStockObject(NULL_BRUSH));

        for (const auto& shape : shapes) {
            switch (shape.type) {
            case 0: // Line
                MoveToEx(hdc, shape.start.x, shape.start.y, NULL);
                LineTo(hdc, shape.end.x, shape.end.y);
                break;
            case 1: // Rectangle
                Rectangle(hdc, shape.start.x, shape.start.y, shape.end.x, shape.end.y);
                break;
            case 2: // Circle
                Ellipse(hdc, shape.start.x, shape.start.y, shape.end.x, shape.end.y);
                break;
            }
        }

        if (isDrawing) {
            switch (currentShape.type) {
            case 0:
                MoveToEx(hdc, currentShape.start.x, currentShape.start.y, NULL);
                LineTo(hdc, currentShape.end.x, currentShape.end.y);
                break;
            case 1:
                Rectangle(hdc, currentShape.start.x, currentShape.start.y, currentShape.end.x, currentShape.end.y);
                break;
            case 2:
                Ellipse(hdc, currentShape.start.x, currentShape.start.y, currentShape.end.x, currentShape.end.y);
                break;
            }
        }

        DeleteObject(pen);
        EndPaint(hwnd, &ps);
        break;
    }
    default:
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }
    return 0;
}
