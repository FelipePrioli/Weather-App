import { Routes } from '@angular/router';
import { SearchComponent } from './features/search/search.component';
import { ForecastComponent } from './features/forecast/forecast.component';
import { FavoritesComponent } from './features/favorites/favorites.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: SearchComponent },
  { path: 'forecast', component: ForecastComponent },
  {
    path: 'favorites',
    component: FavoritesComponent,
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '' },
];
