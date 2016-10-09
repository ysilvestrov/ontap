import { NgModule }      from '@angular/core';
import { FormsModule }   from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { PubsComponent } from './components/pubs/pubs.component';
import { CitiesComponent } from './components/cities/cities.component';
import { BeersComponent } from "./components/beers/beers.component";
import { EPubsComponent } from "./components/epubs/epubs.component";
import { ServesComponent } from "./components/serves/serves.component";
import { BreweriesComponent } from "./components/breweries/breweries.component";
import { UsersComponent } from "./components/users/users.component";
import { LoginComponent } from "./components/login/login.component";
import { PubAdminsComponent } from './components/pubadmins/pubadmins.component';
import { BreweryAdminsComponent } from './components/breweryadmins/breweryadmins.component';
import { HomeComponent } from './components/home/home.component';
import { TooltipContainerComponent, TooltipDirective, TooltipModule, Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        BeersComponent,
        BreweriesComponent,
        CitiesComponent,
        EPubsComponent,
        PubsComponent,
        ServesComponent,
        LoginComponent,
        PubAdminsComponent,
        BreweryAdminsComponent,
        UsersComponent
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        FormsModule,
        TooltipModule, 
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'beers', component: BeersComponent},
            { path: 'breweries', component: BreweriesComponent },
            { path: 'cities', component: CitiesComponent },
            { path: 'epubs', component: EPubsComponent },
            { path: 'pubs', component: PubsComponent },
            { path: 'serves', component: ServesComponent },
            { path: 'users', component: UsersComponent },
            { path: 'pub-admins', component: PubAdminsComponent },
            { path: 'brewery-admins', component: BreweryAdminsComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
})
export class AppModule {
}
