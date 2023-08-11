#include <stdio.h>
#include <windows.h>


int cps = 0;
int delay = 1000;
int keybind;
int screen_width;
int screen_height;
int x;
int y;

void Action(int x, int y)
{
    mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
    mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
    Sleep(delay);
    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
}

int main() {
    printf("    ____             _       ___________      __             \n");
    printf("   / __ \\____ ______(_)___ _/ / ____/ (_)____/ /_____  _____\n");
    printf("  / /_/ / __ `/ ___/ / __ `/ / /   / / / ___/ //_/ _ \\/ ___/\n");
    printf(" / _, _/ /_/ / /__/ / /_/ / / /___/ / / /__/ ,< /  __/ /     \n");
    printf("/_/ |_|\\__,_/\\___/_/\\__,_/_/\\____/_/_/\\___/_/|_|\\___/_/\n");


    printf("How much cps: ");
    scanf("%d", &cps);
    if (cps == 0)
    {
        printf("Invalid input info. \nrestarting the program...\n\n\n");
        Sleep(5000);
        main();
    }
    printf("Delay between clicks (in milliseconds): ");
    scanf("%d", &delay);
    if (delay > 999)
    {
        printf("Invalid input info, maximum amount of delay is 999. \nrestarting the program...\n\n\n");
        Sleep(5000);
        main();
    }
    printf("Keybind Code: ");
    scanf("%x", &keybind);
    if (GetKeyState(keybind) == false) {
        printf("Invalid input info. \nCheck https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes for valid inputs. \nBy example: 0x11");
    }
    
    screen_width = GetSystemMetrics(SM_CXSCREEN);
    screen_height = GetSystemMetrics(SM_CYSCREEN);
    x = screen_width / 2;
    y = screen_height / 2;

    while (1) {
        if ((GetKeyState(keybind) & 0x8000) != 0)
        {
            Action(x, y);
            Sleep(1000 / cps - delay);
        }
    }
}
