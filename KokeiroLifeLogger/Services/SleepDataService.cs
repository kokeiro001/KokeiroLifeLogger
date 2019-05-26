using KokeiroLifeLogger.Repositories;
using KokeiroLifeLogger.Utilities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public class SleepDataService
    {
        private readonly SleepDataRepository sleepDataRepository;

        public SleepDataService(
            SleepDataRepository sleepDataRepository
        )
        {
            this.sleepDataRepository = sleepDataRepository;
        }

        public Task AddAsync(SleepDataEntity entity)
        {
            return sleepDataRepository.AddAsync(entity);
        }

        public async Task<string> GetDiary(DateTime from, DateTime to)
        {
            var data = await sleepDataRepository.GetByDate(from, to);

            if (data.Length == 0)
            {
                return "no sleep data.\n";
            }

            var dateFormat = "yyyy/MM/dd HH:mm";
            var timeSpanFormat = @"hh\hmm\m";

            var text = data.Select(item =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"【睡眠スコア】{item.SleepScore}");
                stringBuilder.AppendLine($"【睡眠時間】{TimeSpan.Parse(item.SleepDuration).ToString(timeSpanFormat)}");
                stringBuilder.AppendLine($"【布団入った時間】{item.InBedDateandTime.AddHours(9).ToString(dateFormat)}");
                stringBuilder.AppendLine($"【布団出た時間】　{item.OutofBedDateandTime.AddHours(9).ToString(dateFormat)}");
                stringBuilder.AppendLine($"【寝るまでにかかった時間】　{TimeSpan.Parse(item.TimeToSleep).ToString(timeSpanFormat)}");
                stringBuilder.AppendLine($"【起きるまでにかかった時間】{TimeSpan.Parse(item.TimeToGetUp).ToString(timeSpanFormat)}");
                stringBuilder.AppendLine($"【平均心拍数】{item.HeartRateAverage}");
                stringBuilder.AppendLine($"【睡眠の規則性】{item.StatusRegularity}");
                stringBuilder.AppendLine($"【睡眠の中断回数】{item.NbInterruptions}");
                stringBuilder.AppendLine();
                return stringBuilder.ToString();
            })
            .JoinString("\n");

            return text;
        }
    }
}
