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

    class TrelloSprint
    {
        private readonly static DateTime Sprint0StartDate = new DateTime(2019, 4, 29);

        public int Number { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public string Name => StartDate.ToString("yyyy MM/dd") + "-" + EndDate.ToString("MM/dd");

        public static TrelloSprint GetSprint(DateTime dateTime)
        {
            var pasedDate = (dateTime - Sprint0StartDate).Days;

            var currentSprint = (pasedDate / 14);

            var currentSprintStartDate = Sprint0StartDate.AddDays(currentSprint * 14);
            var currentSprintEndDate = currentSprintStartDate.AddDays(13);

            return new TrelloSprint
            {
                Number = currentSprint,
                StartDate = currentSprintStartDate,
                EndDate = currentSprintEndDate,
            };
        }

        public static bool TryParseFromName(string sprintName, out TrelloSprint result)
        {
            try
            {
                var split = sprintName.Split(' ');

                var year = int.Parse(split[0]);

                var dateSplit = split[1].Split('-');

                var startSplit = dateSplit[0].Split('/');
                var startMonth = int.Parse(startSplit[0]);
                var startDay = int.Parse(startSplit[1]);
                var startDate = new DateTime(year, startMonth, startDay);

                var endSplit = dateSplit[1].Split('/');
                var endMonth = int.Parse(endSplit[0]);
                var endDay = int.Parse(endSplit[1]);
                var endDate = new DateTime(year, endMonth, endDay).AddDays(1).AddSeconds(-1);

                result = GetSprint(startDate);

                // todo: 期待するスプリントフォーマットか検証する

                return true;
            }
            catch
            {
                result = new TrelloSprint();
                return false;
            }
        }
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
            var todos = ParseTodoTweet(tweet).ToArray();

            if (todos.Length == 0)
            {
                return;
            }

            var jstNow = DateTime.UtcNow.AddHours(9);
            var currentBoard = await GetCurrentBoard(jstNow);

            await currentBoard.Refresh();

            // Progressって名前のリストを取得する。予め用意しておく。
            var progressList = currentBoard.Lists
                .Where(x => x.Name.ToLower() == "progress")
                .FirstOrDefault();

            if (progressList == null)
            {
                return;
            }

            var cardPrefix = jstNow.ToString("MMdd");

            foreach (var todo in todos)
            {
                var cardName = $"{cardPrefix} {todo}";
                await progressList.Cards.Add(cardName);
            }
        }

        /// <summary>
        /// TODO
        /// - やること1
        /// - やること2
        /// みたいなフォーマットのツイートから、「やること1」「やること2」を取得する
        /// </summary>
        private IEnumerable<string> ParseTodoTweet(TweetRequest tweet)
        {
            var lines = tweet.Text.Split('\n');

            if (lines.Length == 0)
            {
                return new string[0];
            }

            if (!lines[0].ToLower().Contains("todo"))
            {
                return new string[0];
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

        /// <summary>
        /// yyyy MM/dd-MMdd
        /// ってフォーマットのボード名の中から、
        /// 現在の日付内のBoardを取得する。
        /// 存在しなかった場合、新規作成する。
        /// </summary>
        private async Task<IBoard> GetCurrentBoard(DateTime jstDateTime)
        {
            var targetSprint = TrelloSprint.GetSprint(jstDateTime);

            var me = await trelloFactory.Me();

            foreach (var board in me.Boards)
            {
                if (!TrelloSprint.TryParseFromName(board.Name, out var result))
                {
                    continue;
                }

                if (result.Number == targetSprint.Number)
                {
                    return board;
                }
            }

            // not found baord. create board.
            var newBaord = await me.Boards.Add(targetSprint.Name);

            await newBaord.Refresh();

            await newBaord.Lists.Refresh();

            var listNames = new string[] { "todo", "progress", "done" };

            // listが少なかったら追加する
            while (newBaord.Lists.Count() < listNames.Length)
            {
                await newBaord.Lists.Add($"tmp{newBaord.Lists.Count()}");
                await newBaord.Lists.Refresh();
            }

            for (int i = 0; i < listNames.Length; i++)
            {
                newBaord.Lists[i].Name = listNames[i];
                await newBaord.Lists.Refresh();
            }

            await me.StarredBoards.Add(newBaord);
            await newBaord.Lists.Refresh();

            return newBaord;
        }
    }
}
