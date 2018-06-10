using System;

namespace Rasteriser.External
{
    public enum QueueStatusFlags : uint
    {
        QS_KEY = 0x1,
        QS_MOUSEMOVE = 0x2,
        QS_MOUSEBUTTON = 0x4,
        QS_MOUSE = (QS_MOUSEMOVE | QS_MOUSEBUTTON),
        QS_INPUT = (QS_MOUSE | QS_KEY),
        QS_POSTMESSAGE = 0x8,
        QS_TIMER = 0x10,
        QS_PAINT = 0x20,
        QS_SENDMESSAGE = 0x40,
        QS_HOTKEY = 0x80,
        QS_REFRESH = (QS_HOTKEY | QS_KEY | QS_MOUSEBUTTON | QS_PAINT),
        QS_ALLEVENTS = (QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY),
        QS_ALLINPUT = (QS_SENDMESSAGE | QS_PAINT | QS_TIMER | QS_POSTMESSAGE | QS_MOUSEBUTTON | QS_MOUSEMOVE | QS_HOTKEY | QS_KEY),
        QS_ALLPOSTMESSAGE = 0x100,
        QS_RAWINPUT = 0x400
    }
}
