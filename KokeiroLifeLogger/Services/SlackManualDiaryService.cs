using SlackAPI;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface ISlackManualDiaryService
    {
        Task<string> GetManualDiary();
    }

    class SlackManualDiaryService : ISlackManualDiaryService
    {
        private readonly string OAuthAccessToken;

        public SlackManualDiaryService(string OAuthAccessToken)
        {
            this.OAuthAccessToken = OAuthAccessToken;
        }

        public async Task<string> GetManualDiary()
        {
            var client = new SlackTaskClient(OAuthAccessToken);

            var channesl = await client.GetChannelListAsync();
            var diaryChannel = channesl.channels
                .Where(x => x.name == "diary")
                .FirstOrDefault();

            if (diaryChannel == null)
            {
                Console.WriteLine("not found diary channel.");
                return null;
            }

            var history = await client.GetChannelHistoryAsync(diaryChannel);

            var yesterday = DateTime.UtcNow.AddDays(-1);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"(debug) DateTime.UtcNow=" + DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));
            stringBuilder.AppendLine($"(debug) yesterday=" + yesterday.ToString("yyyy/MM/dd HH:mm:ss"));

            stringBuilder.AppendLine();

            var targetMessages = history.messages
                .Where(x => string.IsNullOrEmpty(x.subtype))
                .Reverse()
                .ToArray();

            if (targetMessages.Length == 0)
            {
                stringBuilder.AppendLine("(マニュアル日記はありません)");
                return stringBuilder.ToString();
            }

            foreach (var targetMessage in targetMessages)
            {
                var messageUtc = targetMessage.ts.ToUniversalTime();

                if (messageUtc >= yesterday)
                {
                    stringBuilder.AppendLine("(debug messageUtc)) " + messageUtc.ToString("yyyy/MM/dd HH:mm:ss"));

                    var lines = targetMessage.text.Split('\n');

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("<http") && line.EndsWith(">"))
                        {
                            var url = line.TrimStart('<').TrimEnd('>');
                            stringBuilder.AppendLine(url);
                        }
                        else
                        {
                            stringBuilder.AppendLine(line);
                        }
                    }

                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }
    }
}
