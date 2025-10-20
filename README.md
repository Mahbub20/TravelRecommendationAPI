# Strativ .NET Travel Recommendation (Air quality + Temperature)

## Overview
This ASP.NET Core Web API ranks Bangladesh districts by coolest temperature at 14:00 and lowest PM2.5, and gives travel recommendations.

## Requirements
- .NET 7 or 8 SDK

## How to run
1. Clone:
   git clone <your-repo>
2. Build:
   dotnet build
3. Run: Open CMD or terminal on project root folder 

   dotnet run

4. Get the top 10 districts list by this API.

--> GET /api/Districts/top_ten_districts

5. Get the travel recommendation message by the below API.

--> POST /api/Travel/recommendation

Request body:
{
  "currentLat": 23.8103,
  "currentLong": 90.4125,
  "destinationDistrict": "Cox's Bazar",
  "travelDate": "2025-10-20T02:30:42.309Z"
}

## Notes
- Uses Open-Meteo (no API key).

## Libraries
- Microsoft.Extensions.Caching.Memory

