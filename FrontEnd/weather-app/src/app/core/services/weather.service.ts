import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Weather, Forecast } from '../models/weather.model';


@Injectable({ providedIn: 'root' })
export class WeatherService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getWeatherByCity(city: string) {
    const params = new HttpParams().set('city', city);
    return this.http.get<Weather>(`${this.api}/weather`, { params });
  }

  getForecast(city: string) {
  const params = new HttpParams().set('city', city);
  return this.http.get<Forecast[]>(`${this.api}/weather/forecast`, { params });
}

}
