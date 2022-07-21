using EndToEnd.Data.EndToEnd;
using Microsoft.EntityFrameworkCore;

namespace EndToEnd.Data
{
    public class WeatherForecastService
    {
        private readonly EndtoendContext _context;

        public WeatherForecastService(EndtoendContext context )
        {
            _context = context;
        }

        public async Task<List<WeatherForecast>> GetForeCastAsync(String strCurrentUser)
        {
            return await _context.WeatherForecast
                .Where(x => x.UserName == strCurrentUser)
                .AsNoTracking().ToListAsync();
        }
         

    }










    //public class WeatherForecastService
    //{
    //    private static readonly string[] Summaries = new[]
    //    {
    //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //};

    //    public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
    //    {
    //        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //        {
    //            Date = startDate.AddDays(index),
    //            TemperatureC = Random.Shared.Next(-20, 55),
    //            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //        }).ToArray());
    //    }
    //}
}