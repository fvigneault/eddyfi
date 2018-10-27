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

    private static List<Point> GetPoints()
    {
      List<Point> map = LoadMap();

      map.Sort((point1, point2) => point1.Value.CompareTo(point2.Value));
      
      List<Cluster> clusters = GetClusters(map.Skip(31).ToList());

      var tuple = GetPathBetweenPoints(clusters.Select(c => c.Represent).ToList());

      List<Point> path = new List<Point>()
      {
        map.First()
      };

      foreach(Point point in tuple.Item1) {
        Cluster currentCluster = clusters.Find(cluster => cluster.Represent.Equals(point));
        path.AddRange(currentCluster.Path);
      }

      return path;
    }

    class Cluster
    {
      public List<Point> Path;
      public Point Represent;
      public double Value;
    }

    private static List<Cluster> GetClusters(List<Point> points)
    {
      Dictionary<String, List<Point>> clusters = new Dictionary<string, List<Point>>();

      foreach(Point point in points) {
        int xTopCorner = point.X / 200 * 200;
        int yTopCorner = point.Y / 200 * 200;
        String key = xTopCorner + "," + yTopCorner;

        if (!clusters.ContainsKey(key)){
          clusters[key] = new List<Point>();
        }

        clusters[key].Add(point);
      }

      return clusters.Values.Select(BuildCluster).ToList();
    }

    private static Cluster BuildCluster(List<Point> points) {
      Cluster cluster = new Cluster();

      Tuple<List<Point>, double> tuple = GetPathBetweenPoints(points);
      double revenue = tuple.Item1.Sum(point => point.Value);
      double totalDistance = tuple.Item2;
      cluster.Value = revenue - totalDistance * 10;

      cluster.Path = tuple.Item1;

      cluster.Represent = tuple.Item1.First();

      return cluster;
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