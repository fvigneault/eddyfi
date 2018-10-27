using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContestantApp.Utilities
{
  public class TaskUtilities
  {
    public static Task GetSTATask( Action _action )
    {
      TaskCompletionSource<Object> source = new TaskCompletionSource<Object>();

      Thread thread = new Thread( () =>
      {
        try
        {
          _action();
          source.SetResult( null );
        }
        catch ( Exception ex )
        {
          source.SetException( ex );
        }
      } );

      thread.SetApartmentState( ApartmentState.STA );
      thread.Start();

      return source.Task;
    }
  }
}