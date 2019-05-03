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

            var yesterday = DateTime.UtcNow.AddHours(9).AddDays(-1);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"(debug) DateTime.UtcNow=" + DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));

            stringBuilder.AppendLine($"(debug) DateTime.UtcNow.AddHours(9)=" + DateTime.UtcNow.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));
            stringBuilder.AppendLine($"(debug) yesterday=" + yesterday.ToString("yyyy/MM/dd HH:mm:ss"));

            stringBuilder.AppendLine();

            var targetMessages = history.messages
                .Where(x => string.IsNullOrEmpty(x.subtype))
                .Reverse();

            foreach (var targetMessage in targetMessages)
            {
                if (targetMessage.ts >= yesterday)
                {
                    stringBuilder.AppendLine("(debug ts)) " + targetMessage.ts.ToString("yyyy/MM/dd HH:mm:ss"));

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
