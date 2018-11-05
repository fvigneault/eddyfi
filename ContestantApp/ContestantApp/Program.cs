using System;
using System.Collections.Generic;
using System.Diagnostics;
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
      return points.Zip(points.Skip(1).Append(points.First()), (point1, point2) => GetDistance(point1, point2) * -10 + point1.Value).Sum();
    }

    public static List<Point> LoadMap()
    {
      using (StreamReader r = new StreamReader("OfficialMap.json"))
      {
        string json = r.ReadToEnd();
        return JsonConvert.DeserializeObject<List<Point>>(json);
      }
    }
    
    class Link
    {
      public Point p1;
      public Point p2;
      public double distance;
    }

    private static Tuple<Link, Link> SplitLink(Link link, Point point)
    {
      return new Tuple<Link, Link>(
        new Link
        {
          p1 = link.p1,
          p2 = point,
          distance = GetDistance(link.p1, point)
        },
        new Link
        {
          p1 = point,
          p2 = link.p2,
          distance = GetDistance(point, link.p2)
        });
    }

    private static Link FindLinkToSplit(Point point, List<Link> existingLinks)
    {
      Link linkMostEasilySplittable = null;
      double smallestDifferenceOfSplittedLinks = Double.PositiveInfinity;

      foreach (var link in existingLinks)
      {
        var newLinks = SplitLink(link, point);
        var currentDistanceOfSplittedLinks = newLinks.Item1.distance + newLinks.Item2.distance;
        var currentDifferenceOfSplittedLinks = currentDistanceOfSplittedLinks - link.distance;
        if (currentDifferenceOfSplittedLinks < smallestDifferenceOfSplittedLinks)
        {
          linkMostEasilySplittable = link;
          smallestDifferenceOfSplittedLinks = currentDifferenceOfSplittedLinks;
        }
      }

      return linkMostEasilySplittable;
    }

    private static List<Link> GetCircularPathInternal(Point point, List<Link> existingLinks)
    {
      if (existingLinks.Count == 0)
      {
        return new List<Link>
        {
          new Link
          {
            p1 = point,
            p2 = point,
            distance = 0
          }
        };
      }

      var linkToSplit = FindLinkToSplit(point, existingLinks);
      var newLinks = SplitLink(linkToSplit, point);

      Debug.Assert(existingLinks.Remove(linkToSplit));
      existingLinks.Add(newLinks.Item1);
      existingLinks.Add(newLinks.Item2);

      return existingLinks;
    }

    private static List<Link> GetCircularPath(List<Point> points)
    {
      List<Link> links = new List<Link>();
      foreach (var point in points)
      {
        links = GetCircularPathInternal(point, links);
      }
      return links;
    }
    
    private static List<Point> GetPathFromLinks(List<Link> links)
    {
      var path = new List<Point>();

      var firstLink = links.First();
      path.Add(firstLink.p1);
      path.Add(firstLink.p2);
      links.RemoveAt(0);

      while (links.Count != 0)
      {
        var currentLink = links.First();
        links.RemoveAt(0);

        var indexP1 = path.IndexOf(currentLink.p1);
        var indexP2 = path.IndexOf(currentLink.p2);

        if (indexP1 == -1 && indexP2 == -1)
        {
          links.Add(currentLink);
        }
        else if (indexP1 != -1 && indexP2 == -1)
        {
          if (indexP1 + 1 == path.Count)
          {
            path.Add(currentLink.p2);
          }
          else
          {
            path.Insert(indexP1 + 1, currentLink.p2);
          }
        }
        else if (indexP1 == -1 && indexP2 != -1)
        {
          path.Insert(indexP2, currentLink.p1);
        }
      }

      var indexOfFirstPoint = path.FindIndex(point => point.Value == 0);
      var bestPath = path.Skip(indexOfFirstPoint).ToList();
      bestPath.AddRange(path.Take(indexOfFirstPoint).ToList());
      return bestPath;
    }

    private static List<Point> GetPoints()
    {
      List<Point> map = LoadMap();

      map.Sort((point1, point2) => point1.Value.CompareTo(point2.Value));

      var payingMapWithFirst = new List<Point>(map);
      payingMapWithFirst.RemoveRange(1, 30);

      payingMapWithFirst = payingMapWithFirst.Take(170).ToList();

      var links = GetCircularPath(payingMapWithFirst);
      var path = GetPathFromLinks(links);

      return path;
    }
    
    private static double GetDistance(Point point1, Point point2)
    {
      return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
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