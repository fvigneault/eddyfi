using ContestantApp.Utilities;
using SolutionViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ContestantApp.Solutions
{
  class GreedyDetours : ISolver
  {
    private class Link
    {
      public Point P1 { get; set; }

      public Point P2 { get; set; }

      public double Distance { get; set; }
    }

    private readonly List<Point> points;

    public GreedyDetours(List<Point> points)
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

    private Link FindLinkToSplit(Point point, List<Link> existingLinks)
    {
      Link linkMostEasilySplittable = null;
      double smallestDifferenceOfSplittedLinks = Double.PositiveInfinity;

      foreach (var link in existingLinks)
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

      return linkMostEasilySplittable;
    }

    private List<Link> GetCircularPathInternal(Point point, List<Link> existingLinks)
    {
      if (existingLinks.Count == 0)
      {
        return new List<Link>
        {
          new Link
          {
            P1 = point,
            P2 = point,
            Distance = 0
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

    private List<Link> GetCircularPath(List<Point> points)
    {
      List<Link> links = new List<Link>();
      foreach (var point in points)
      {
        links = GetCircularPathInternal(point, links);
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
