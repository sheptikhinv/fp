using System.Drawing;
using TagsCloudContainer.Core.CoordinateGenerators;

namespace TagsCloudContainer.Core.Tests.CoordinateGenerators;

[TestFixture]
public class SpiralCoordinateGeneratorTests
{
    [Test]
    public void GetNextPosition_ProducesDifferentPoints_OverTime_Test()
    {
        var center = new Point(0, 0);
        var gen = new SpiralCoordinateGenerator(1.0);

        var set = new HashSet<Point>();
        for (var i = 0; i < 10; i++)
            set.Add(gen.GetNextPosition());

        Assert.That(set.Count, Is.GreaterThan(1), "Expected more than one unique point after 10 calls");
    }
}
