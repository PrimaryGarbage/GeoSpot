using NetTopologySuite.Geometries;

namespace GeoSpot.Persistence.Entities.Factories;

public static class GeographyFactory
{
    public static Point CreatePoint(double latitude, double longitude)
    {
        return new Point(longitude, latitude) { SRID = 4326 };
    }
}