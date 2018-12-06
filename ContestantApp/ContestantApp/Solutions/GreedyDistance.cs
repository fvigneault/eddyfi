using System;
using System.Collections.Generic;
using System.Linq;
using ContestantApp.Utilities;
using SolutionViewer.Utilities;

namespace ContestantApp.Solutions
{
  public class GreedyDistance : ISolver
  {
    private readonly List<Point> points;

    public GreedyDistance(List<Point> points)
    {
      this.points = new List<Point>(points);
    }

    public List<Point> GetPath()
    {
      return GetPathBetweenPoints(points).Item1;
    }

    private static Tuple<List<Point>, double> GetPathBetweenPoints(List<Point> points)
    {
      List<Point> path = new List<Point>()
      {
        points.First()
      };

      double cumulativeDistance = 0;

      points.Remove(points.First());

      while (points.Count > 0)
      {
        var currentPoint = path.Last();

        Point closestPoint = points.First();
        double smallestDistance = currentPoint.DistanceFrom(points.First());
        foreach (Point nextPoint in points)
        {
          var distance = currentPoint.DistanceFrom(nextPoint);
          if (distance < smallestDistance)
          {
            closestPoint = nextPoint;
            smallestDistance = distance;
          }
        }

        path.Add(closestPoint);
        points.Remove(closestPoint);
        cumulativeDistance += smallestDistance;
      }

      return Tuple.Create(path, cumulativeDistance);
    }
  }
}
