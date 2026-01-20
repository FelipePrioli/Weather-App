import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { WeatherService } from '../../core/services/weather.service';
import { Forecast } from '../../core/models/weather.model';

@Component({
  selector: 'app-forecast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './forecast.component.html',
  styleUrls: ['./forecast.component.scss']
})
export class ForecastComponent implements OnInit {

  city = '';
  forecast: Forecast[] = [];
  loading = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private weatherService: WeatherService
  ) {}

  ngOnInit(): void {
    console.log('[ForecastComponent] ngOnInit');

    this.route.queryParams.subscribe(params => {
      console.log('[ForecastComponent] Query params:', params);

      this.city = params['city'];
      console.log('[ForecastComponent] City recebida:', this.city);

      if (this.city) {
        this.loadForecast();
      } else {
        console.warn('[ForecastComponent] City não informada');
      }
    });
  }

  loadForecast() {
    console.log('[ForecastComponent] Iniciando loadForecast para:', this.city);

    this.loading = true;
    this.error = '';

    this.weatherService.getForecast(this.city).subscribe({
      next: (data) => {
        console.log('[ForecastComponent] Dados recebidos do backend:', data);
        console.log('[ForecastComponent] Tipo do retorno:', typeof data);
        console.log('[ForecastComponent] É array?', Array.isArray(data));

        this.forecast = data;
        this.loading = false;

        console.log('[ForecastComponent] Forecast atribuído:', this.forecast);
      },
      error: (err) => {
        console.error('[ForecastComponent] Erro ao buscar forecast:', err);

        this.error = 'Erro ao carregar previsão';
        this.loading = false;
      },
      complete: () => {
        console.log('[ForecastComponent] Observable finalizado');
      }
    });
  }
}
