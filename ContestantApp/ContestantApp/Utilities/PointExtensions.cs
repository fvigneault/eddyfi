using SolutionViewer.Utilities;
using System;

namespace ContestantApp.Utilities
{
  public static class PointExtensions
  {
    public static double DistanceFrom(this Point point1, Point point2)
    {
      return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
    }
  }
}
