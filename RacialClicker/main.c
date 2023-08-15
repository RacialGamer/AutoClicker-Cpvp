#include <stdio.h>
#include <windows.h>


int Cps = 0;
int Delay = 1000;
int Keybind;
int ScreenWidth;
int ScreenHeight;
int x;
int y;
boolean CpsSet;
boolean DelaySet;
boolean KeybindSet;

void action(int x, int y)
{
    mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
    mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
    Sleep(Delay);
    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
}

int main() 
{
    printf("    ____             _       ___________      __             \n");
    printf("   / __ \\____ ______(_)___ _/ / ____/ (_)____/ /_____  _____\n");
    printf("  / /_/ / __ `/ ___/ / __ `/ / /   / / / ___/ //_/ _ \\/ ___/\n");
    printf(" / _, _/ /_/ / /__/ / /_/ / / /___/ / / /__/ ,< /  __/ /     \n");
    printf("/_/ |_|\\__,_/\\___/_/\\__,_/_/\\____/_/_/\\___/_/|_|\\___/_/\n");

    while (!CpsSet) 
    {
        printf("How much cps: ");
        scanf("%d", &Cps);
        if (Cps == 0)
        {
            printf("Invalid input info.\n\n");
        }
        else
        {
            CpsSet = true;
        }
    }
    while (!DelaySet) {
        printf("Delay between clicks (in milliseconds): ");
        scanf("%d", &Delay);
        if (Delay > 999) 
        {
            printf("Invalid input info, maximum amount of delay is 999.\n\n");
        }
        else
        {
            DelaySet = true;
        }
    }
    while (!KeybindSet) 
    {
        printf("Keybind Code: ");
        scanf("%x", &Keybind);
        UINT doesKeycodeExist = MapVirtualKey(Keybind, MAPVK_VSC_TO_VK);
        if (doesKeycodeExist == 0) 
        {
            printf("Invalid input info,\nCheck https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes for keybinds. By example: 0x11\n\n");
        } else 
        {
            KeybindSet = true;
        }
    }


        ScreenWidth = GetSystemMetrics(SM_CXSCREEN);
        ScreenHeight = GetSystemMetrics(SM_CYSCREEN);
        x = ScreenWidth / 2;
        y = ScreenHeight / 2;

        while (1) 
        {
            if ((GetKeyState(Keybind) & 0x8000) != 0) 
            {
                action(x, y);
                Sleep(1000 / Cps);
            }
        }
}
