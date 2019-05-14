using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokeiroLifeLogger.Repositories;
using KokeiroLifeLogger.Utilities;

namespace KokeiroLifeLogger.Services
{
    public interface IWithingsSleepService
    {
        Task AddAsync(WithingsSleepEntity entity);
        Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to);
        Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to);
        Task<string> GetDiaryData(DateTime from, DateTime to);
    }

    public class WithingsSleepService : IWithingsSleepService
    {
        class SleepLength
        {
            public DateTime IntoTime;
            public DateTime OutTime;

            public TimeSpan Lenght => OutTime - IntoTime;
        }

        private readonly IWithingsSleepRepository withingsSleepRepository;

        public WithingsSleepService(
            IWithingsSleepRepository withingsSleepRepository
        )
        {
            this.withingsSleepRepository = withingsSleepRepository;
        }

        public async Task AddAsync(WithingsSleepEntity entity)
        {
            await withingsSleepRepository.AddAsync(entity);
        }

        public Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to)
        {
            return withingsSleepRepository.GetIntoBedDataByDate(from, to);
        }

        public Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to)
        {
            return withingsSleepRepository.GetOutBedDataByDate(from, to);
        }

        public async Task<string> GetDiaryData(DateTime from, DateTime to)
        {
            var intoData = await GetIntoBedDataByDate(from, to);
            var outData = await GetOutBedDataByDate(from, to);

            var data = intoData.Concat(outData)
                .OrderBy(x => x.Date)
                .GroupBy(x => x.Date.ToString())
                .Where(x => x.Count() == 1)
                .SelectMany(x => x);

            var intoItem = default(WithingsSleepEntity);
            var lenghtList = new List<SleepLength>();

            foreach (var item in data)
            {
                if (intoItem != null)
                {
                    if (item.Action == "out bed")
                    {
                        lenghtList.Add(new SleepLength
                        {
                            IntoTime = intoItem.Date,
                            OutTime = item.Date,
                        });
                        intoItem = null;
                    }
                    else if (item.Action == "into bed")
                    {
                        intoItem = item;
                    }
                }
                else if (item.Action == "into bed")
                {
                    intoItem = item;
                }
            }

            var format = "yyyy/MM/dd HH:mm";
            var text = lenghtList
                .Select(x => $"[寝]{x.IntoTime.ToString(format)}\n↓\n[起]{x.OutTime.ToString(format)}\n ({x.Lenght.ToString(@"hh\hmm\m")})\n")
                .JoinString("\n");

            if (intoItem != null)
            {
                text += $"[寝]{intoItem.Date.ToString(format)}\n";
            }

            return text;
        }
    }
}
