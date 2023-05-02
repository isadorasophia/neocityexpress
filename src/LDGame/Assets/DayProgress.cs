using Murder.Utilities;

namespace LDGame.Assets
{
    public record struct DayProgress()
    {
        public int Day = 0;
        
        /// <summary>
        /// Range of 0 to 1 of how much distance was covered by CAR.
        /// </summary>
        public float DistancePercentile = 0;
        
    }
}
