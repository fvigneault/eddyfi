using SolutionViewer.Utilities;
using System.Collections.Generic;

namespace ContestantApp.Solutions
{
  public interface ISolver
  {
    List<Point> GetPath();
  }
}
