using ConsoleApp1;

var testIntDatas = new List<TestIntValue>
{
    new(1, "1", "1", "1")
};
var testDoubleDatas = new List<TestDoubleValue>
{
    new(12345.12, "12345.12", "12345.12", "12345.12"),
    new(12345.123, "12345.123", "12345.123", "12345.12"),
    new(12345.1234, "12345.1234", "12345.123", "12345.12"),
    new(123456.12, "12345.612e+01", "12345.612e+01", "12345.61e+01"),
    new(123456.123, "12345.6123e+01", "12345.612e+01", "12345.61e+01"),
    new(123456.1234, "12345.6123e+01", "12345.612e+01", "12345.61e+01"),
    new(12.12345678, "12.1235", "12.123", "12.12"),
    new(0.01, "0.01", "0.01", "0.01"),
    new(0.001, "0.001", "0.001", "1e-03"),
    new(0.012, "0.012", "0.012", "0.01"),
    new(0.0001, "0.0001", "1e-04", "1e-04"),
    new(0.0012, "0.0012", "0.001", "1.2e-3"),
    new(0.0123, "0.0123", "0.012", "0.01"),
    new(0.00001, "1e-05", "1e-05", "1e-05"),
    new(0.00012, "0.0001", "1.2e-04", "1.2e-04"),
    new(0.00123, "0.0012", "0.001", "1.23e-03"),
    new(0.01234, "0.0123", "0.012", "0.01"),
    new(0.0000123456789, "1.2346e-05", "1.235e-05", "1.23e-05")
};

foreach (var data in testIntDatas)
{
    Console.WriteLine(
        $"{data.Value.Format(FormatType.None)}, {data.Value.Format(FormatType.NineFour)}, {data.Value.Format(FormatType.EightThree)}, {data.Value.Format(FormatType.SevenTwo)}");
}

foreach (var data in testDoubleDatas)
{
    Console.WriteLine(
        $"{data.Value.Format(FormatType.None)}, {data.Value.Format(FormatType.NineFour)}, {data.Value.Format(FormatType.EightThree)}, {data.Value.Format(FormatType.SevenTwo)}");
}

public record TestIntValue(int Value, string NineFourFormat, string EightThreeFormat, string SevenTwoFormat);

public record TestDoubleValue(double Value, string NineFourFormat, string EightThreeFormat, string SevenTwoFormat);