using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiAgentSystem
{
    class StartUp
    {
        #region Matrix
        static Random rand = new Random();
        static int width, height;

        public void first()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.CursorVisible = false;
            int[] kind, position;

            initialize(out kind, out position);

            while (true)
            {
                printColumns(kind, position);
                Thread.Sleep(50);

                for (int x = 0; x < width; x++)
                {
                    if (position[x] == height)
                        position[x] = 0;
                    else
                        position[x]++;
                }
            }
        }

        static char AsciiCharacter
        {
            get
            {
                int t = rand.Next(10);
                if (t <= 2)
                    // returns a number
                    return (char)('0' + rand.Next(10));
                else if (t <= 4)
                    // small letter
                    return (char)('a' + rand.Next(27));
                else if (t <= 6)
                    // capital letter
                    return (char)('A' + rand.Next(27));
                else
                    // any ascii character
                    return (char)(rand.Next(32, 255));
            }
        }

        static void printColumns(int[] kind, int[] position)
        {
            char c = 'l';
            int halfHeight = height / 2;
            ConsoleColor tmpColor = Console.ForegroundColor;
            for (int x = 0; x < width; x++)
            {
                Console.SetCursorPosition(x, position[x]);

                if (Console.CursorTop == 0 || kind[x] == 1)
                {
                    if (rand.Next(width) == 1 || kind[x] == 1)
                    {
                        c = AsciiCharacter;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(c);

                        c = AsciiCharacter;

                        kind[x] = 1;
                    }
                    else
                        kind[x] = 0;

                    if (Console.CursorTop == height)
                        kind[x] = 0;
                        c = AsciiCharacter;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(c);

                    c = AsciiCharacter;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    c = AsciiCharacter;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(c);

                    c = AsciiCharacter;
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                if (position[x] < height - 2)
                {
                    Console.SetCursorPosition(x, position[x] + 2);
                    Console.Write(c);
                }

                if (position[x] >= halfHeight)
                {
                    Console.SetCursorPosition(x, position[x] - halfHeight);
                    Console.Write(' ');
                }
                else if (position[x] <= halfHeight)
                {
                    Console.SetCursorPosition(x, position[x] + halfHeight+1);
                    Console.Write(' ');
                }
                Console.ForegroundColor = tmpColor;
            }
        }

        static void initialize(out int[] kind, out int[] position)
        {
            width = Console.WindowWidth - 1;
            height = Console.WindowHeight - 1;

            // Initializes x to the width of the console window.
            position = new int[width];
            kind = new int[width];

            // Create startpositions
            for (int i = 0; i < width; i++)
            {
                position[i] = rand.Next(height);
            }

            //
            for (int i = 0; i < width; i++)
            {
                kind[i] = 0;
            }
        }
        #endregion

        #region Aparture

        public static void Aparture()
        {
            ConsoleColor tmpForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(
@"

               ',-:;//;:=
          . :H@@@MM@M#H/.,+%;,
       ,/X+ +M@@M@MM%=,.%HMMM@X/,
     -+@MM; $M@@MH+-,;XMMMM@MMMM@+-
    ;@M@@M- XM@X;. -+XXXXXHHH@M@M#@/.
  ,%MM@@MH ,@%=           .---=-=:=,.
  =@#@@@MX .,             -%HX$$%%%+;
 =-./@M@M$                 .;@MMMM@MM:
 X@/ -@MM/                   .+MM@@@M$
,@M@H  :@:                   . =X#@@@@-
,@@@MMX,  .                      ;@M@M=
.H@@@@M@+,                       .%#$. 
 /MMMM@MMH/.                      _ =,___ ___ ___ _____ _   _ ___ ___ 
  /%+%$XHH@$=                ,   /_\ | _ \ __| _ \_   _| | | | _ \ __|
   .=--------.            -%H.  / _ \|  _/ _||   / | | | |_| |   / _|
   .%MM@@@HHHXX$$$%+-  .:$MMX  /_/ \_\_| |___|_|_\ |_|  \___/|_|_\___|
     =XMMM@MM@MM#H;. -+HMM@M+      _  _  _  _  _ ___ _  _ ___ __ __
       =%@M@M#@$-.=$@MM@@@M;   |  |_||_)/ \|_)|_| | / \|_) | |_ (_ 
         ,:+$+-,/H#MMMMMMM@=   |__| ||_)\_/| \| | | \_/| \_|_|____)
               =++%%%%+/:-.    


Press any key to start testing.");
            Console.ForegroundColor = tmpForegroundColor;
        }

        #endregion
    }
}
