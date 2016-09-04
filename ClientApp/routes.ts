import { RouterConfig } from '@angular/router';
import { Pubs } from './components/pubs/pubs';
import { Cities } from './components/cities/cities';
import { Beers } from "./components/beers/beers";
import { EPubs } from "./components/epubs/epubs";

export const routes: RouterConfig = [
    { path: '', redirectTo: 'pubs', pathMatch: 'full' },
    { path: 'pubs', component: Pubs },
    { path: 'cities', component: Cities },
    { path: 'beers', component: Beers },
    { path: 'epubs', component: EPubs },
    { path: '**', redirectTo: 'pubs' }
];