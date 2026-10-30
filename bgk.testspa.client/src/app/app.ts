import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public isAuthenticated = false;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<{ name?: string; username?: string }>('/auth/user').subscribe({
      next: () => {
        this.isAuthenticated = true;
        this.getForecasts();
      },
      error: (error) => {
        if (error.status === 401) {
          const returnUrl = `${location.pathname}${location.search}${location.hash}`;
          location.assign(`/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`);
          return;
        }

        console.error('Nie udało się zweryfikować sesji użytkownika.', error);
      },
    });
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  protected readonly title = signal('bgk.testspa.client');
}
