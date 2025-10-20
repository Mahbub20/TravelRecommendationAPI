# Strativ .NET Travel Recommendation (Air quality + Temperature)

## Overview
This ASP.NET Core Web API ranks Bangladesh districts by coolest temperature at 14:00 and lowest PM2.5, and returns travel recommendations based on current location, destination district and travel date/time.

## Key features:
- Ranks districts by temperature at 14:00 and PM2.5 (air quality).
- Provides travel recommendation messages (considering distance, weather and air quality).
- Uses Open-Meteo (no API key) for weather data.
- In-memory caching for improved performance.

## Prerequisites
- .NET SDK 7.x or 8.x (installed and on PATH)
- Git

## Quick setup
1. Clone
   git clone <repo-url>
   cd "TravelRecommendationApi"

2. Install required NuGet packages (if not already in project file)
   dotnet restore
   dotnet add package Microsoft.Extensions.Caching.Memory

3. Build
   dotnet build

4. Run (development)
   dotnet run --project . 
   # or, if the solution has multiple projects, use the specific project folder:
   # cd src/YourApiProject && dotnet run

5. Open browser or call the API on the configured port (default is usually 5000/5001 for HTTPS).


## API Endpoints

1) GET top 10 districts
- URL:
  GET /api/Districts/top_ten_districts
- Response (example):
  [
    { "district": "Cox's Bazar", "temp_at_14": 29.3, "pm25": 12.4, "score": 1.23 },
    ...
  ]

2) POST travel recommendation
- URL:
  POST /api/Travel/recommendation
- Request body (application/json):
  {
    "currentLat": 23.8103,
    "currentLong": 90.4125,
    "destinationDistrict": "Cox's Bazar",
    "travelDate": "2025-10-20T02:30:42.309Z"
  }

- Response (example):
  {
    "recommended": true,
    "message": "Cox's Bazar looks good on 2025-10-20: cooler at 14:00 and low PM2.5. Travel advisable."
  }

## Notes about external services
- Open-Meteo: used for weather data (no API key). Confirm endpoints in your code (likely GET to Open-Meteo endpoints).
- Caching: project uses Microsoft.Extensions.Caching.Memory to cache API responses (install package if missing).



