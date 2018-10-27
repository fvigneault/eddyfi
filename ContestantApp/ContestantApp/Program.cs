using System;
using System.Collections.Generic;
using ContestantApp.Utilities;
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

    private static List<Point> GetPoints()
    {
      return new List<Point>();
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