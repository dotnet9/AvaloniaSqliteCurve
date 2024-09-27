using ConsoleApp1;

namespace TestProject1;

[TestClass]
public class ScaleUnitTest
{
    [TestMethod]
    public void Test_DataScale_Success()
    {
        var min = 0;
        var max = 420;

        var items = new List<CompareItem>()
        {
            new(-200, -20, 100),
            new(2, -20, 100),
            new(20, -20, 100),
            new(80, -20, 100),
            new(180, -20, 100),
            new(-0, -20, 20),
            new(-300, -200, 100000),
            new(300, -200, 100000),
        };
        foreach (var item in items)
        {
            var scaleValue = ScaleHelper.ScaleDataToRange(item.value, item.Min, item.Max, min, max);
            Assert.AreEqual(item.value < item.Min, scaleValue < min);
            Assert.AreEqual(item.value > item.Min, scaleValue > min);
            Assert.AreEqual(item.value < item.Max, scaleValue < max);
            Assert.AreEqual(item.value > item.Max, scaleValue > max);
        }
    }
}

public static class ScaleHelper
{
    // 计算缩放因子并缩放数据点  
    public static double ScaleDataToRange(double value, double minValue, double maxValue, double displayMin,
        double displayMax)
    {
        // 避免除以零  
        if (maxValue == minValue) return displayMin;

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
}

public record CompareItem(double value, double Min, double Max);