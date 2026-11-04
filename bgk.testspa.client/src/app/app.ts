import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  public list: string[] = [];
  public isAuthenticated = false;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.http.get<{ name?: string; username?: string }>('auth/user').subscribe({
      next: () => {
        this.isAuthenticated = true;
        this.cdr.detectChanges();
        this.getData();
      },
      error: (error) => {
        if (error.status === 401) {
          const returnUrl = `${location.pathname}${location.search}${location.hash}`;
          location.assign(`auth/login?returnUrl=${encodeURIComponent(returnUrl)}`);
          return;
        }

        console.error('Nie udało się zweryfikować sesji użytkownika.', error);
      },
    });
  }

  getData() {
    this.http.get<string[]>('test').subscribe(
      (result) => {
        this.list = result;
        this.cdr.detectChanges();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  protected readonly title = signal('bgk.testspa.client');
}
