using ContestantApp.Utilities;
using SolutionViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ContestantApp.Solutions
{
  class GreedyDetours2 : ISolver
  {
    private class Link
    {
      public Point P1 { get; set; }

      public Point P2 { get; set; }

      public double Distance { get; set; }
    }

    private class Detour
    {
      public Link InitialLink { get; set; }

      public Link FirstLink { get; set; }

      public Link SecondLink { get; set; }

      public double Cost {
        get
        {
          return FirstLink.Distance + SecondLink.Distance - InitialLink.Distance;
        }
      }
    }

    private readonly List<Point> points;

    public GreedyDetours2(List<Point> points)
    {
      this.points = new List<Point>(points);
    }

    public List<Point> GetPath()
    {
      var links = GetCircularPath(points);
      var path = GetPathFromLinks(links);

      return path;
    }

    private Tuple<Link, Link> SplitLink(Link link, Point point)
    {
      return new Tuple<Link, Link>(
        new Link
        {
          P1 = link.P1,
          P2 = point,
          Distance = link.P1.DistanceFrom(point)
        },
        new Link
        {
          P1 = point,
          P2 = link.P2,
          Distance = point.DistanceFrom(link.P2)
        });
    }

    private Detour MakeDetour(Link initialLink, Point newPoint)
    {
      return new Detour
      {
        InitialLink = initialLink,
        FirstLink = new Link
        {
          P1 = initialLink.P1,
          P2 = newPoint,
          Distance = initialLink.P1.DistanceFrom(newPoint)
        },
        SecondLink = new Link
        {
          P1 = newPoint,
          P2 = initialLink.P2,
          Distance = newPoint.DistanceFrom(initialLink.P2)
        }
      };
    }

    private Detour FindSmallestDetour(List<Link> links, Point point)
    {
      Link linkMostEasilySplittable = null;
      double smallestDifferenceOfSplittedLinks = Double.PositiveInfinity;

      foreach (var link in links)
      {
        var newLinks = SplitLink(link, point);
        var currentDistanceOfSplittedLinks = newLinks.Item1.Distance + newLinks.Item2.Distance;
        var currentDifferenceOfSplittedLinks = currentDistanceOfSplittedLinks - link.Distance;
        if (currentDifferenceOfSplittedLinks < smallestDifferenceOfSplittedLinks)
        {
          linkMostEasilySplittable = link;
          smallestDifferenceOfSplittedLinks = currentDifferenceOfSplittedLinks;
        }
      }

      return MakeDetour(linkMostEasilySplittable, point);
    }

    private Detour FindSmallestDetour(List<Link> links, List<Point> nonVisitedPoints)
    {
      Detour smallestDetour = null;
      foreach (Point point in nonVisitedPoints)
      {
        var detour = FindSmallestDetour(links, point);
        if (smallestDetour == null || detour.Cost < smallestDetour.Cost)
        {
          smallestDetour = detour;
        }
      }
      return smallestDetour;
    }

    private List<Link> GetCircularPath(List<Point> points)
    {
      List<Link> links = new List<Link>();
      links.Add(new Link
      {
        P1 = points.First(),
        P2 = points.First(),
        Distance = 0
      });
      List<Point> nonVisitedPoints = new List<Point>(points.Skip(1));

      while (nonVisitedPoints.Count > 0)
      {
        // Find point with lowest detour from current circular path
        Detour smallestDetour = FindSmallestDetour(links, nonVisitedPoints);

        // Add detour to current circular path
        Debug.Assert(links.Remove(smallestDetour.InitialLink));
        links.Add(smallestDetour.FirstLink);
        links.Add(smallestDetour.SecondLink);

        // Remove point from nonVisitedPoints
        Debug.Assert(nonVisitedPoints.Remove(smallestDetour.FirstLink.P2));
      }

      return links;
    }

    private List<Point> GetPathFromLinks(List<Link> links)
    {
      var path = new List<Point>();

      var firstLink = links.First();
      path.Add(firstLink.P1);
      path.Add(firstLink.P2);
      links.RemoveAt(0);

      while (links.Count != 0)
      {
        var currentLink = links.First();
        links.RemoveAt(0);

        var indexP1 = path.IndexOf(currentLink.P1);
        var indexP2 = path.IndexOf(currentLink.P2);

        if (indexP1 == -1 && indexP2 == -1)
        {
          links.Add(currentLink);
        }
        else if (indexP1 != -1 && indexP2 == -1)
        {
          if (indexP1 + 1 == path.Count)
          {
            path.Add(currentLink.P2);
          }
          else
          {
            path.Insert(indexP1 + 1, currentLink.P2);
          }
        }
        else if (indexP1 == -1 && indexP2 != -1)
        {
          path.Insert(indexP2, currentLink.P1);
        }
      }

      var indexOfFirstPoint = path.FindIndex(point => point.Value == 0);
      var bestPath = path.Skip(indexOfFirstPoint).ToList();
      bestPath.AddRange(path.Take(indexOfFirstPoint).ToList());
      return bestPath;
    }
  }
}
