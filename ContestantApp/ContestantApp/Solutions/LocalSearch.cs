using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantApp.Utilities;
using SolutionViewer.Utilities;

namespace ContestantApp.Solutions
{
  public class LocalSearch : ISolver
  {
    private class Link
    {
      public Point P1 { get; set; }

      public Point P2 { get; set; }

      public double Distance { get; set; }
    }

    private readonly List<Point> points;

    public LocalSearch(List<Point> points)
    {
      this.points = new List<Point>(points);
    }

    public List<Point> GetPath()
    {
      List<Point> pathFromGreedyDistance = new GreedyDetours(points).GetPath();

      bool noModification = false;
      while (noModification)
      {
        noModification = true;
        for (int i = 0; i < pathFromGreedyDistance.Count; ++i)
        {
          var point1 = pathFromGreedyDistance[i];
          var point2 = pathFromGreedyDistance[(i + 1) % pathFromGreedyDistance.Count];
          var point3 = pathFromGreedyDistance[(i + 2) % pathFromGreedyDistance.Count];
          var point4 = pathFromGreedyDistance[(i + 3) % pathFromGreedyDistance.Count];

          var distanceBetweenMiddlePoints = point2.DistanceFrom(point3);
          var costCurrentForPointPath = point1.DistanceFrom(point2) + distanceBetweenMiddlePoints + point3.DistanceFrom(point4);
          var costPathWithSwitchedMiddlePoints = point1.DistanceFrom(point3) + distanceBetweenMiddlePoints + point2.DistanceFrom(point4);

          if (costPathWithSwitchedMiddlePoints < costCurrentForPointPath)
          {
            noModification = false;

            pathFromGreedyDistance[(i + 1) % pathFromGreedyDistance.Count] = point3;
            pathFromGreedyDistance[(i + 2) % pathFromGreedyDistance.Count] = point2;
          }
        }
      }

      return pathFromGreedyDistance;
    }
  }
}
