﻿using Microsoft.Azure;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    class BlogPvStringLoader
    {
        public async Task<string> LoadAsync()
        {
            var date = DateTime.UtcNow.AddHours(9).AddDays(-1);


            var sb = new StringBuilder();

            sb.AppendLine("-------------------------------------"); // TODO: このhrもUtilityから取得するようにする

            // TODO: 並列処理するようにする
            var pvReader = new GoogleAnalyticsReader();

            // はてブは１日前のデータはまだ取得できないので、更に前日のデータを取得する
            var hatebuPv = await pvReader.ReadPv(CloudConfigurationManager.GetSetting("HatebuViewId"), date.AddDays(-1));
            sb.AppendLine($"はてなブログのPV数：{hatebuPv}");

            var qiitaPv = await pvReader.ReadPv(CloudConfigurationManager.GetSetting("QiitaViewId"), date);
            sb.AppendLine($"QiitaのPV数：{qiitaPv}");
            sb.AppendLine();
            
            return sb.ToString();
        }
    }
}
