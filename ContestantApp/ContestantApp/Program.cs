using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContestantApp.Utilities;
using Newtonsoft.Json;
using SolutionViewer.Model;
using SolutionViewer.Utilities;

namespace ContestantApp
{
  internal class Program
  {
    private static void Main()
    {
      const String TeamId = "5bd1e020ce65420001eccb91";

      Validate( TeamId, GetPoints() );
    }

    public static List<Point> LoadMap()
    {
      using (StreamReader r = new StreamReader("OfficialMap.json"))
      {
        string json = r.ReadToEnd();
        return JsonConvert.DeserializeObject<List<Point>>(json);
      }
    }

    private enum Direction
    {
      Up,
      Down,
      Left,
      Right
    }

    private static List<Point> GetPoints()
    {
      List<Point> map = LoadMap();

      map.Sort((point1, point2) => point1.Value.CompareTo(point2.Value));

      var payingMap = map.Skip(31).ToList();

      Dictionary<String, List<Point>> clusters = GetClusters(payingMap, 2, 4);

      List<List<Point>> clustersOrderedList = new List<List<Point>>()
      {
        clusters["0,250"],
        clusters["0,0"],
        clusters["500,0"],
        clusters["500,250"],
        clusters["500,500"],
        clusters["500,750"],
        clusters["0,750"],
        clusters["0,500"]
      };

      clustersOrderedList[0].Sort((point1, point2) => -point1.X.CompareTo(point2.X));
      clustersOrderedList[1].Sort((point1, point2) => point1.X.CompareTo(point2.X));
      clustersOrderedList[2].Sort((point1, point2) => point1.X.CompareTo(point2.X));
      clustersOrderedList[3].Sort((point1, point2) => -point1.X.CompareTo(point2.X));
      clustersOrderedList[4].Sort((point1, point2) => point1.X.CompareTo(point2.X));
      clustersOrderedList[5].Sort((point1, point2) => -point1.X.CompareTo(point2.X));
      clustersOrderedList[6].Sort((point1, point2) => -point1.X.CompareTo(point2.X));
      clustersOrderedList[7].Sort((point1, point2) => point1.X.CompareTo(point2.X));



      List<Point> path = new List<Point>()
      {
        map.First()
      };

      path.AddRange(clustersOrderedList.SelectMany(x => x));

      return path;
    }

    class Cluster
    {
      public List<Point> Path;
      public Point Represent;
      public double Value;
    }

    private static Dictionary<String, List<Point>> GetClusters(List<Point> points, int nbClustersHorizontal, int nbClustersVertical)
    {
      Dictionary<String, List<Point>> clusters = new Dictionary<string, List<Point>>();

      foreach(Point point in points) {
        int xTopCorner = point.X / (1000 / nbClustersHorizontal) * (1000 / nbClustersHorizontal);
        int yTopCorner = point.Y / (1000 / nbClustersVertical) * (1000 / nbClustersVertical);
        String key = xTopCorner + "," + yTopCorner;

        if (!clusters.ContainsKey(key)){
          clusters[key] = new List<Point>();
        }

        clusters[key].Add(point);
      }

      return clusters;
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
        double smallestDistance = GetDistance(currentPoint, points.First());
        foreach (Point nextPoint in points)
        {
          var distance = GetDistance(currentPoint, nextPoint);
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

    private static double GetDistance(Point point1, Point point2)
    {
      return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
    }

    private static void Validate( String _teamId, List<Point> _points )
    {
      PointManager pointManager = new PointManager();

      TaskUtilities.GetSTATask( () =>
      {
        Double score;
        Boolean success = pointManager.SetSolutionPoints( _teamId, _points, out score, out String errorMsg );

        if ( success )
        {
          Console.WriteLine( $@"Score: {score}" );
        }
        else
        {
          Console.WriteLine( $@"Message: {errorMsg}" );
        }
      } );

      pointManager.WriteSolutionToFile( _teamId + ".json", _points );
    }
  }
}