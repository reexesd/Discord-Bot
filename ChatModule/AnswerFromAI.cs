using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Discord_Bot.ChatModule
{
    public partial class AnswerFromAI
    {
        private readonly string _answer;
        private readonly Regex _codeRegex = CodeBlockRegex();
        private  string _answerWOCode = string.Empty;
        private bool _codeExist;
        private string[] _partsOfAnswer = [];
        private List<CodeBlock> _codeBlocks = [];

        public bool IsHugeAnswer { get => _answer.Length > 2000; }
        public bool CodeExist { get => _codeExist; }
        public List<CodeBlock> CodeBlocks { get => _codeBlocks; }
        public string[] PartsOfAnswer { get => _partsOfAnswer; }
        public string Content { get => _answer; }  
        public string ContentWOCode { get => _answerWOCode; }

        public AnswerFromAI(string answerFromApi)
        {
            _answer = answerFromApi;
            Init();
        }

        private void Init()
        {
            _codeExist = IsCodeExist();
            _codeBlocks = GetCodeBlocks();
            _answerWOCode = GetAnswerWOCode();
            _partsOfAnswer = CutAnswerToParts();
        }

        private string[] CutAnswerToParts()
        {
            int msgPartsCount = _answer.Length / 2000;
            string[] partialAnswers = new string[msgPartsCount + 1];

            int currentPos = 0;
            int nextPos = currentPos + 2000;
            for (int i = 0; i < partialAnswers.Length; i++)
            {
                if (currentPos + 2000 < _answer.Length)
                    partialAnswers[i] = _answer[currentPos..nextPos];
                else
                    partialAnswers[i] = _answer[currentPos..];

                currentPos = nextPos;
                nextPos += currentPos;
            }

            return partialAnswers;
        }

        private bool IsCodeExist()
        {
            var match = _codeRegex.Match(_answer);

            if (match.Success)
                return true;

            return false;
        }

        private List<CodeBlock> GetCodeBlocks()
        {
            var matches = _codeRegex.Matches(_answer);

            if (matches.Count < 1)
                return [];

            List<CodeBlock> codeBlocks = [];
            string ext;
            string code;

            for (int i = 0; i < matches.Count; i++)
            {
                code = matches[i].Groups[2].Value;
                ext = GetFileExtOfLanguage(matches[i].Groups[1].Value);
                codeBlocks.Add(new CodeBlock(code, ext));
            }

            return codeBlocks;
        }

        private string GetAnswerWOCode()
        {
            int iterator = 1;
            return _codeRegex.Replace(_answer, (match) => { return $"> code{iterator++}"; });
        }

        private static string GetFileExtOfLanguage(string language)
        {
            language = language.ToLower();
            return language switch
            {
                "assembly" => "asm",
                "bash" => "bat",
                "c" => "c",
                "cpp" => "cpp",
                "csharp" => "cs",
                "c#" => "cs",
                "dart" => "dart",
                "go" => "go",
                "haskell" => "hs",
                "html" => "html",
                "java" => "java",
                "javascript" => "js",
                "julia" => "jl",
                "json" => "json",
                "kotlin" => "kt",
                "lua" => "lua",
                "matlab" => "m",
                "php" => "php",
                "powershell" => "ps1",
                "python" => "py",
                "r" => "r",
                "ruby" => "rb",
                "rust" => "rs",
                "swift" => "swift",
                "typescript" => "ts",

                _ => "txt"
            };
        }

        [GeneratedRegex(@"`{3}(.*?)\n(.*?)`{3}", RegexOptions.Singleline)]
        private static partial Regex CodeBlockRegex();
    }
}
