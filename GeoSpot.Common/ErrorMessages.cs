namespace GeoSpot.Common;

public static class ErrorMessages
{
    public static string FailedToFindById<TObject>(object id) => $"Failed to find {typeof(TObject).Name} with the given ID: {id}";
}