import { RouterConfig } from '@angular/router';
import { Pubs } from './components/pubs/pubs';
import { Cities } from './components/cities/cities';
import { Beers } from "./components/beers/beers";
import { EPubs } from "./components/epubs/epubs";
import { Serves } from "./components/serves/serves";
import { Breweries } from "./components/breweries/breweries";
import { Home } from './components/home/home';

export const routes: RouterConfig = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'pubs', component: Pubs },
    { path: 'cities', component: Cities },
    { path: 'beers', component: Beers },
    { path: 'epubs', component: EPubs },
    { path: 'serves', component: Serves },
    { path: 'breweries', component: Breweries },
    { path: '**', redirectTo: 'home' }
];
