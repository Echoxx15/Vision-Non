using System;

using IKapCDotNet;
using IKapBoardDotNet;

namespace IKNS
{
    public class IKUtils
    {
        // 用户输入回车键后退出程序。
        //
        // Users enter Enter to exit the program.
        public static void pressEnterToExit()
        {
            Console.Write("\nPress Enter key to exit...\n");
            while (Console.ReadKey().Key != ConsoleKey.Enter) {; }
            Environment.Exit(1);
        }

        // 打印错误信息后退出程序。
        //
        // Print error message and exit the program.
        public static void printErrorAndExit(uint errc)
        {

            if (errc != IKapC.ITKSTATUS_OK)
            {
                Console.Write("Error Code: {0}.\n", errc.ToString("x8"));
                IKapC.ItkManTerminate();
                pressEnterToExit();
            }
        }

        // 判断IKapCDotNet函数是否成功调用
        //
        // Determine whether the function is called successfully.
        public static void CheckIKapC(uint errc)
        {
            if (IKapC.ITKSTATUS_OK != errc)
            {
                printErrorAndExit(errc);
            }
        }

        // 打印 IKapBoard 错误信息后退出程序。
        //
        // Print IKapBoard error message and exit the program.
        public static void printIKapBoardErrorAndExit()
        {
            IKAPERRORINFO errc = new IKAPERRORINFO();
            IKapBoard.IKapGetLastError(errc, true);

            Console.Write($"IKapBoard:Index: {errc.uBoardIndex}, error code: {errc.uErrorCode:X4}");

            pressEnterToExit();
        }

        /* @brief：判断 IKapBoard 函数是否成功调用。
         * @param[in] errc：函数返回值。
         *
         * @brief：Determine whether the IKapBoard function is called successfully.
         * @param[in] errc：Function return value. */
        public static void CheckIKapBoard(int errc)
        {
            if (errc != IKapBoard.IK_RTN_OK)
            {
                printIKapBoardErrorAndExit();
            }
        }

        public static void WaitEnterKeyInput() {
            Console.Write("\nWaiting the Enter key to input...\n");
            while (Console.ReadKey().Key != ConsoleKey.Enter) {; }
        }
    }
}
