﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.ChatModule
{
    public class CodeBlock(string code, string fileExt)
    {
        private readonly string _code = code;
        private readonly string _fileExt = fileExt;

        public string Code { get => _code; }
        public string FileExt { get => _fileExt; }
    }
}
