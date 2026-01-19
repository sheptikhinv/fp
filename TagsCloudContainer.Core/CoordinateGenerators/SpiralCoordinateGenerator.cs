using System.Drawing;

namespace TagsCloudContainer.Core.CoordinateGenerators;

public class SpiralCoordinateGenerator : ICoordinateGenerator
{
    private double _angleRadians;
    private readonly double _stepRadians;

    public SpiralCoordinateGenerator(double stepRadians)
    {
        _angleRadians = 0;
        _stepRadians = stepRadians;
    }

    public Point GetNextPosition()
    {
        var radius = _stepRadians * _angleRadians;
        var x = radius * Math.Cos(_angleRadians);
        var y = radius * Math.Sin(_angleRadians);

        _angleRadians += _stepRadians;

        return new Point((int)x, (int)y);
    }
}