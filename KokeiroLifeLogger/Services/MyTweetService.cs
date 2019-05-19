using Manatee.Trello;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public class TweetRequest
    {
        public string Text { get; set; }
    }

    public interface IMyTweetService
    {
        Task Run(TweetRequest tweet);
    }

    public class MyTweetService : IMyTweetService
    {
        private readonly ITrelloFactory trelloFactory;

        public MyTweetService(
            ITrelloFactory trelloFactory
        )
        {
            this.trelloFactory = trelloFactory;
        }

        public async Task Run(TweetRequest tweet)
        {
            var todos = PasrseTodoTweet(tweet).ToArray();

            if (todos.Length == 0)
            {
                return;
            }

            var currentBoard = await GetCurrentBoard();

            if (currentBoard == null)
            {
                return;
            }

            await currentBoard.Refresh();

            var progressList = currentBoard.Lists
                .Where(x => x.Name.ToLower() == "progress")
                .FirstOrDefault();

            if (progressList == null)
            {
                return;
            }

            var jstNow = DateTime.UtcNow.AddHours(9);
            var date = jstNow.ToString("MMdd");

            foreach (var todo in todos)
            {
                var cardName = $"{date} {todo}";
                await progressList.Cards.Add(cardName);
            }
        }

        private IEnumerable<string> PasrseTodoTweet(TweetRequest tweet)
        {
            var lines = tweet.Text.Split('\n');

            if (lines.Length == 0)
            {
                return null;
            }

            if (!lines[0].ToLower().Contains("todo"))
            {
                return null;
            }

            var todo = new List<string>();
            foreach (var line in lines.Skip(1))
            {
                var trim = line.Trim();
                if (trim.StartsWith("-"))
                {
                    todo.Add(line.Trim('-', ' '));
                }
            }

            return todo;
        }

        private async Task<IBoard> GetCurrentBoard()
        {
            Func<string, (DateTime? start, DateTime? end)> parseBoardDate = boardName => 
            {
                try
                {
                    var split = boardName.Split(' ');

                    var year = int.Parse(split[0]);

                    var dateSplit = split[1].Split('-');

                    var startSplit = dateSplit[0].Split('/');
                    var startMonth = int.Parse(startSplit[0]);
                    var startDay = int.Parse(startSplit[1]);
                    var start = new DateTime(year, startMonth, startDay);

                    var endSplit = dateSplit[1].Split('/');
                    var endMonth = int.Parse(endSplit[0]);
                    var endDay = int.Parse(endSplit[1]);
                    var end = new DateTime(year, endMonth, endDay).AddDays(1).AddSeconds(-1);

                    return (start, end);
                }
                catch 
                {
                    return (null, null);
                }
            };

            var jstNow = DateTime.UtcNow.AddHours(9);

            var me = await trelloFactory.Me();

            foreach (var board in me.Boards)
            {
                var (startDate, endDate) = parseBoardDate(board.Name);

                if (!startDate.HasValue || !endDate.HasValue)
                {
                    continue;
                }

                if (jstNow >= startDate && jstNow <= endDate)
                {
                    return board;
                }
            }

            return null;
        }
    }
}
