using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class WeatherAPIScript : MonoBehaviour
{
    public string latitude;
    public string longitude;
    public string apiKey;
    public TextMeshProUGUI weatherText;

    private void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=kr";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError($"Error fetching weather data: {webRequest.error}");
            }
            else
            {
                ProcessWeatherData(webRequest.downloadHandler.text);
            }
        }
    }

    void ProcessWeatherData(string jsonData)
    {
        WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonData);
        DisplayWeatherInfo(weatherData);
    }

    void DisplayWeatherInfo(WeatherData weatherData)
    {
        weatherText.text = $"Location: {weatherData.name} ({weatherData.coord.lat}, {weatherData.coord.lon})\n" +
                           $"Weather ID: {weatherData.weather[0].id}\n" +
                           $"Main: {weatherData.weather[0].main}\n" +
                           $"Description: {weatherData.weather[0].description}\n" +
                           $"Icon ID: {weatherData.weather[0].icon}\n\n" +
                           $"Temperature: {weatherData.main.temp} ¡ÆC\n" +
                           //$"Feels Like: {weatherData.main.feels_like} ¡ÆC\n" +
                           //$"Pressure: {weatherData.main.pressure} hPa\n" +
                           //$"Humidity: {weatherData.main.humidity}%\n" +
                           $"Min Temp: {weatherData.main.temp_min} ¡ÆC\n" +
                           $"Max Temp: {weatherData.main.temp_max} ¡ÆC\n" +
                           //$"Sea Level Pressure: {weatherData.main.sea_level} hPa\n" +
                           //$"Ground Level Pressure: {weatherData.main.grnd_level} hPa\n" +
                           $"Visibility: {weatherData.visibility} meters\n\n" +
                           $"Wind Speed: {weatherData.wind.speed} m/s\n" +
                           $"Wind Degree: {weatherData.wind.deg}¡Æ\n" +
                           //$"Wind Gust: {weatherData.wind.gust} m/s\n\n" +
                           $"Cloudiness: {weatherData.clouds.all}%\n" +
                           $"Rain 1h: {weatherData.rain?._1h ?? 0} mm\n" +
                           $"Rain 3h: {weatherData.rain?._3h ?? 0} mm\n" +
                           $"Snow 1h: {weatherData.snow?._1h ?? 0} mm\n" +
                           $"Snow 3h: {weatherData.snow?._3h ?? 0} mm\n\n" +
                           $"Sunrise: {UnixTimeStampToDateTime(weatherData.sys.sunrise)}\n" +
                           $"Sunset: {UnixTimeStampToDateTime(weatherData.sys.sunset)}";
    }


    DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}

[Serializable]
public class WeatherData
{
    public Coord coord;
    public Weather[] weather;
    public Main main;
    public int visibility;
    public Wind wind;
    public Clouds clouds;
    public Rain rain;
    public Snow snow;
    public int dt;
    public Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;
}

[Serializable]
public class Coord
{
    public float lon;
    public float lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class Main
{
    public float temp;
    public float feels_like;
    public float temp_min;
    public float temp_max;
    public int pressure;
    public int humidity;
    public int sea_level; // Ãß°¡µÊ
    public int grnd_level; // Ãß°¡µÊ
}

[Serializable]
public class Wind
{
    public float speed;
    public int deg;
    public float gust; // Ãß°¡µÊ
}

[Serializable]
public class Clouds
{
    public int all;
}

[Serializable]
public class Rain
{
    public float? _1h;
    public float? _3h;
}

[Serializable]
public class Snow
{
    public float? _1h;
    public float? _3h;
}

[Serializable]
public class Sys
{
    public int type;
    public int id;
    public string country;
    public int sunrise;
    public int sunset;
}
