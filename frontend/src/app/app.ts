import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Root component. It only hosts the router: each page (login, register, grocery list)
 * is rendered inside <router-outlet />.
 */
@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {}
