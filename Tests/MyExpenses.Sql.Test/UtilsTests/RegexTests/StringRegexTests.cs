using MyExpenses.Sql.Utils.Regex;

namespace MyExpenses.Sql.Test.UtilsTests.RegexTests;

public class StringRegexTests
{
    [Fact]
    private void SplitUpperCaseWordTest()
    {
        const string input = "AliceBlue";
        const string output = "Alice Blue";

        var result = input.SplitUpperCaseWord();

        Assert.Equivalent(output, result);
    }
}