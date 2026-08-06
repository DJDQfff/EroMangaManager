using System.Net;

namespace Server
{
    public class LogEntry
    {
        public int StatusCode { get; set; }
        public required IPAddress IPAddress { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public required string Method { get; set; }
        public PathString Path { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string ElapsedDisplay => $"{ElapsedTime.TotalMilliseconds:F0} ms";
        public string Level { get; set; } = "INFO"; // INFO, WARN, ERROR

        public override string ToString()
        {
            return $"{IPAddress} {Method} {Path} → {StatusCode} {ElapsedDisplay}";
        }
    }
}
