import { RouterConfig } from '@angular/router';
import { Pubs } from './components/pubs/pubs';
import { Cities } from './components/cities/cities';
import { Beers } from "./components/beers/beers";
import { EPubs } from "./components/epubs/epubs";
import { Home } from './components/home/home';
import { FetchData } from './components/fetch-data/fetch-data';
import { Counter } from './components/counter/counter';

export const routes: RouterConfig = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'counter', component: Counter },
    { path: 'fetch-data', component: FetchData },
    { path: 'pubs', component: Pubs },
    { path: 'cities', component: Cities },
    { path: 'beers', component: Beers },
    { path: 'epubs', component: EPubs },
    { path: '**', redirectTo: 'home' }
];
