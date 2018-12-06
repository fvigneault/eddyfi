using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContestantApp.Solutions;
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
      PointManager pointManager = new PointManager();

      Directory.CreateDirectory("results");
      
      var points = GetPoints();
      var score = CalculateScore(points);
      
      pointManager.WriteSolutionToFile("results/" + score + ".json", points);
      Console.WriteLine(score);

      //Validate(TeamId, points);
      Console.WriteLine("End.");
      Console.ReadKey();
    }

    public static double CalculateScore(List<Point> points)
    {
      return points.Zip(points.Skip(1).Append(points.First()), (point1, point2) => point1.DistanceFrom(point2) * -10 + point1.Value).Sum();
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

      List<Point> points = new List<Point>(map);
      points.Sort((point1, point2) => point1.Value.CompareTo(point2.Value));
      points.RemoveRange(1, 30);

      ISolver solver = new GreedyDetours(points);

      return solver.GetPath();
    }

    private static void Validate( String _teamId, List<Point> _points )
    {
      PointManager pointManager = new PointManager();

      Double finalScore = 0.0;

      var uiTask = TaskUtilities.GetSTATask( () =>
      {
        Double score;
        Boolean success = pointManager.SetSolutionPoints( _teamId, _points, out score, out String errorMsg );

        if ( success )
        {
          finalScore = score;
          Console.WriteLine( $@"Score: {score}" );
        }
        else
        {
          Console.WriteLine( $@"Message: {errorMsg}" );
        }
        Directory.CreateDirectory("results");
        pointManager.WriteSolutionToFile("results/" + finalScore + ".json", _points);
      } );
    }
  }
}