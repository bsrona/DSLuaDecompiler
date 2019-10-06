﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SoulsFormats;

namespace luadec
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream stream = File.OpenRead(args[0]))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                var lua = new LuaFile(br);
                IR.Function main = new IR.Function();
                //LuaDisassembler.DisassembleFunction(lua.MainFunction);
                LuaDisassembler.GenerateIR(main, lua.MainFunction);
                Console.OutputEncoding = Encoding.GetEncoding("shift_jis");
                Console.WriteLine(main.ToString());
            }
        }
    }
}