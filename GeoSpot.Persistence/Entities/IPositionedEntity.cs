using NetTopologySuite.Geometries;

namespace GeoSpot.Persistence.Entities;

public interface IPositionedEntity
{
    double Longitude { get; set; }
    
    double Latitude { get; set; }
    
    Point Position { get; set; }
}