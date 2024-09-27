using ConsoleApp1;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void Test_DataScale_Success()
    {
        var min = 0;
        var max = 420;

        var v1 = ScaleDataFromRange(-200, -20, 100, min, max);
        var v2 = ScaleDataFromRange(2, -20, 100, min, max);
        var v3 = ScaleDataFromRange(20, -20, 100, min, max);
        var v4 = ScaleDataFromRange(80, -20, 100, min, max);
        var v5 = ScaleDataFromRange(180, -20, 100, min, max);
        var v6 = ScaleDataFromRange(0, -20, 20, min, max);
        var v7 = ScaleDataFromRange(-300, -200, 100000, min, max);
        var v8 = ScaleDataFromRange(300, -200, 100000, min, max);


        Assert.IsTrue(v1 > 0);
    }

    // 计算缩放因子并缩放数据点  
    double ScaleDataFromRange(double value, double minValue, double maxValue, double displayMin, double displayMax)
    {
        // 避免除以零  
        if (Math.Abs(maxValue - minValue) < 0.001) return displayMin;

        // 计算原始数据的范围  
        var range = maxValue - minValue;

        // 计算显示范围  
        var displayRange = displayMax - displayMin;

        // 计算缩放因子  
        var scaleFactor = displayRange / range;

        // 计算偏移量以确保数据正确对齐  
        var offset = displayMin - minValue * scaleFactor;

        // 应用缩放和偏移  
        return value * scaleFactor + offset;
    }

    [TestMethod]
    public void Test_DataFormat_Success()
    {
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
            new(0.0012, "0.0012", "0.001", "1.2e-03"),
            new(0.0123, "0.0123", "0.012", "0.01"),
            new(0.00001, "1e-05", "1e-05", "1e-05"),
            new(0.00012, "0.0001", "1.2e-04", "1.2e-04"),
            new(0.00123, "0.0012", "0.001", "1.23e-03"),
            new(0.01234, "0.0123", "0.012", "0.01"),
            new(0.0000123456789, "1.2346e-05", "1.235e-05", "1.23e-05")
        };

        foreach (var data in testIntDatas)
        {
            Assert.AreEqual(data.Value.ToString(), data.Value.Format(FormatType.None));
            Assert.AreEqual(data.NineFourFormat, data.Value.Format(FormatType.NineFour));
            Assert.AreEqual(data.EightThreeFormat, data.Value.Format(FormatType.EightThree));
            Assert.AreEqual(data.SevenTwoFormat, data.Value.Format(FormatType.SevenTwo));
        }

        foreach (var data in testDoubleDatas)
        {
            Assert.AreEqual(data.Value.ToString("#########0.#################"),
                data.Value.Format(FormatType.None));
            Assert.AreEqual(data.NineFourFormat, data.Value.Format(FormatType.NineFour));
            Assert.AreEqual(data.EightThreeFormat, data.Value.Format(FormatType.EightThree));
            Assert.AreEqual(data.SevenTwoFormat, data.Value.Format(FormatType.SevenTwo));
        }
    }
}

public record TestIntValue(int Value, string NineFourFormat, string EightThreeFormat, string SevenTwoFormat);

public record TestDoubleValue(double Value, string NineFourFormat, string EightThreeFormat, string SevenTwoFormat);