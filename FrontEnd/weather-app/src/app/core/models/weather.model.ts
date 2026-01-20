export interface Weather {
  city: string;
  temperature: number;
  tempMin: number;
  tempMax: number;
  humidity: number;
  description: string;
  icon: string;
}

export interface Forecast {
  date: string;        // DateTime → string
  tempMin: number;
  tempMax: number;
  description: string;
  icon: string;
}
