using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurističkaOrganizacija.GUI
{
    static class TextBoxPlaceholder
    {
        const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        // showWhenFocused=false => placeholder hides on focus (recommended)
        public static void SetPlaceholder(TextBox tb, string text, bool showWhenFocused = false)
        {
            SendMessage(tb.Handle, EM_SETCUEBANNER,
                showWhenFocused ? (IntPtr)1 : IntPtr.Zero, text);
        }
    }
    internal class Placeholders
    {
    }
}
