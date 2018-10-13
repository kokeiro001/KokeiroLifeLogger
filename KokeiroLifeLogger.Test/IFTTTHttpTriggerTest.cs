using KokeiroLifeLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KokeiroLifeLogger.Test
{
    public class IFTTTHttpTriggerTest
    {
        [Theory]
        [InlineData("カッコてすと{はい | いいえ}なのだ。https://t.co/First https://t.co/Second", "カッコてすと{はい | いいえ}なのだ。")]
        [InlineData(@"某所にてアドン(コケいろさん@kokeiro0_00 )、エルザ(kokeiroさん@kokeiro) 、Otemoto(コケいろさんのご友人)で、すしざんまいポーズ！ https://t.co/Test", @"某所にてアドン(コケいろさん@kokeiro0_00 )、エルザ(kokeiroさん@kokeiro) 、Otemoto(コケいろさんのご友人)で、すしざんまいポーズ！")]
        [InlineData(@"Unity無料アセット「Hoge」Ver.2.0をリリース https://t.co/HogeFuga #hash #unity https://t.co/FooBar", @"Unity無料アセット「Hoge」Ver.2.0をリリース #hash #unity")]
        [InlineData(@"", @"")]
        [InlineData(null, @"")]
        [InlineData("   ", @"")]
        public void RemoveHttpTest(string input, string expected)
        {
            var actual = StringUtility.RemoveHttp(input);
            actual.Is(expected);
        }
    }
}
