using System;
using KokeiroLifeLogger.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KokeiroLifeLogger.Test
{
    [TestClass]
    public class IFTTTHttpTriggerTest
    {
        [DataTestMethod]
        [DataRow("スゴイけどこの仕事は{できない | やりたくない}って思った。https://t.co/mYWYZUTLeB https://t.co/n8nkD4FvVn", "スゴイけどこの仕事は{できない | やりたくない}って思った。")]
        [DataRow(@"ホココスにてプルシュカ(碧さん@Kiyosi060 )、サーバルちゃん(灰色はむさん@8116x8116) 、ナナチ(碧さんご友人)、私のボ卿で、すしざんまいポーズ！そして親子のツーショットですよ、素晴らしいですね…… https://t.co/N9XTQjF45c", @"ホココスにてプルシュカ(碧さん@Kiyosi060 )、サーバルちゃん(灰色はむさん@8116x8116) 、ナナチ(碧さんご友人)、私のボ卿で、すしざんまいポーズ！そして親子のツーショットですよ、素晴らしいですね……")]
        [DataRow(@"モノビット、Unity無料アセット「VR Voice Chat」正式版となるVer.2.0をリリース https://t.co/8w985QKTNz #panora #unity https://t.co/GS5Up1EHwa", @"モノビット、Unity無料アセット「VR Voice Chat」正式版となるVer.2.0をリリース #panora #unity")]
        [DataRow(@"", @"")]
        [DataRow(null, @"")]
        [DataRow("   ", @"")]
        public void RemoveHttpTest(string input, string expected)
        {
            var actual = StringUtility.RemoveHttp(input);
            Assert.AreEqual(expected, actual);
        }
    }
}
