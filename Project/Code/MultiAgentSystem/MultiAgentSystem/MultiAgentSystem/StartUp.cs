﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiAgentSystem
{
    class StartUp
    {
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
            char c;
            ConsoleColor tmpColor = Console.ForegroundColor;
            for (int x = 0; x < width; x++)
            {
                Console.SetCursorPosition(x, position[x]);

                c = AsciiCharacter;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(c);

                c = AsciiCharacter;
                Console.ForegroundColor = ConsoleColor.Green;

                if (position[x] < height - 2)
                {
                    Console.SetCursorPosition(x, position[x] + 2);
                    Console.Write(c);
                }

                if (position[x] >= height / 2)
                {
                    Console.SetCursorPosition(x, position[x] - (height / 2));
                    Console.Write(' ');
                }
                else if (position[x] <= height / 2)
                {
                    Console.SetCursorPosition(x, position[x] + (height / 2)+1);
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
    }
}