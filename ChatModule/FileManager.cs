using Discord;
using System.Text;

namespace Discord_Bot.ChatModule
{
    public class FileManager
    {
        private readonly UTF8Encoding _encoder = new();

        public async Task SendMessageWithCodeAttachmentsAsync(string message, List<CodeBlock> codeBlocks, ITextChannel channel)
        {
            List<FileAttachment> attachments = [];
            List<MemoryStream> streams = [];

            string code;
            string ext;
            int iterator = 1;

            foreach (var block in codeBlocks)
            {
                code = block.Code;
                ext = block.FileExt;

                MemoryStream ms = new();
                streams.Add(ms);
                byte[] buffer = _encoder.GetBytes(code);
                await ms.WriteAsync(buffer);
                attachments.Add(new(ms, $"code{iterator++}.{ext}"));
            }

            await channel.SendFilesAsync(attachments, message);

            for(int i = 0; i < streams.Count; i++)
                streams[i].Close();
        }
    }
}
