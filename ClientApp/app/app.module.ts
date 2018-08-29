import { APP_INITIALIZER, Injectable, NgModule } from '@angular/core';
import { FormsModule }   from "@angular/forms";
import { RouterModule } from "@angular/router";
import { UniversalModule } from "angular2-universal";
import { AppComponent } from "./components/app/app.component"
import { SortByTap } from "./components/app/sortbytap.pipe"
import { NavMenuComponent } from "./components/navmenu/navmenu.component";
import { PubsComponent } from "./components/pubs/pubs.component";
import { PubComponent } from "./components/pub/pub.component";
import { CitiesComponent } from "./components/cities/cities.component";
import { BeersComponent } from "./components/beers/beers.component";
import { EPubsComponent } from "./components/epubs/epubs.component";
import { BreweriesComponent } from "./components/breweries/breweries.component";
import { UsersComponent } from "./components/users/users.component";
import { LoginComponent } from "./components/login/login.component";
import { LocaleComponent } from "./components/locale/locale.component";
import { PubAdminsComponent } from "./components/pubadmins/pubadmins.component";
import { BreweryAdminsComponent } from "./components/breweryadmins/breweryadmins.component";
import { TapsComponent } from "./components/taps/taps.component";
import { PricesComponent } from "./components/prices/prices.component";
import { KegsComponent } from "./components/kegs/kegs.component";
import { BeerKegsComponent } from "./components/beerkegs/beerkegs.component";
import { BeerKegsOnTapComponent } from "./components/beerkegsontap/beerkegsontap.component";
import { BeerKegWeightsComponent } from "./components/beerkegweights/beerkegweights.component";
import { HomeComponent } from "./components/home/home.component";
import { SelectorComponent } from "./components/selector/selector.component";
import { PrintComponent } from "./components/print/print.component";
import { TooltipContainerComponent, TooltipDirective, TooltipModule, Ng2BootstrapModule, AlertModule  } from "ng2-bootstrap/ng2-bootstrap";
import { LocaleModule, LocalizationModule, LocaleService, LocalizationService } from 'angular2localization';
import { Ng2CloudinaryModule } from 'ng2-cloudinary';
import { FileUploadModule } from 'ng2-file-upload';
import { SelectModule } from 'angular2-select';
import { BootstrapModalModule } from 'ng2-bootstrap-modal';

/**
 * Advanced initialization.
 * 
 * With these settings, translation file will be loaded before the app.
 */
@Injectable()
export class LocalizationConfig {

    constructor(public locale: LocaleService, public localization: LocalizationService) { }

    load(): Promise<any> {

        if (typeof (document) == "undefined") {
            this.locale.enableCookie = false;
            this.locale.enableLocalStorage = false;
        }

        // Adds the languages (ISO 639 two-letter or three-letter code).
        this.locale.addLanguages(["uk", "ru", "en"]);

        // Required: default language, country (ISO 3166 two-letter, uppercase code) and expiry (No days). If the expiry is omitted, the cookie becomes a session cookie.
        // Selects the default language and country, regardless of the browser language, to avoid inconsistencies between the language and country.
        this.locale.definePreferredLocale("uk", "UK", 30);

        // Optional: default currency (ISO 4217 three-letter code).
        this.locale.definePreferredCurrency("UAH");

        // Initializes LocalizationService: asynchronous loading.
        if (typeof (document) == "undefined") {
            this.localization
                .translationProvider("https://ontap.in.ua/resources/locale-");
            // Required: initializes the translation provider with the given path prefix.
        } else {
            this.localization
                .translationProvider("./resources/locale-");
        }

        var promise: Promise<any> = new Promise((resolve: any) => {
            this.localization.translationChanged.subscribe(() => {
                resolve(true);
            });
        });

        this.localization.updateTranslation(); // Need to update the translation.

        return promise;
    }
}

/**
 * Aot compilation requires a reference to an exported function.
 */
export function initLocalization(localizationConfig: LocalizationConfig): Function {
    return () => localizationConfig.load();
}

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
        PubComponent,
        LoginComponent,
        PubAdminsComponent,
        BreweryAdminsComponent,
        UsersComponent,
        TapsComponent,
        PricesComponent,
        KegsComponent,
        BeerKegsComponent,
        BeerKegsOnTapComponent,
        BeerKegWeightsComponent,
        SelectorComponent,
        LocaleComponent,
        PrintComponent,
        SortByTap,
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        FormsModule,
        TooltipModule,
        AlertModule, 
        BootstrapModalModule,
        Ng2CloudinaryModule,
        FileUploadModule,
        SelectModule,
        LocaleModule.forRoot(), // New instance of LocaleService.
        LocalizationModule.forRoot(), // New instance of LocalizationService
        RouterModule.forRoot([
            { path: "", redirectTo: "home", pathMatch: "full" },
            { path: "home", component: HomeComponent },
            { path: "beers", component: BeersComponent},
            { path: "breweries", component: BreweriesComponent },
            { path: "cities", component: CitiesComponent },
            { path: "epubs", component: EPubsComponent },
            { path: "pubs", component: PubsComponent },
            { path: "taps", component: TapsComponent },
            { path: "prices", component: PricesComponent },
            { path: "kegs", component: KegsComponent },
            { path: "beerkegs", component: BeerKegsComponent },
            { path: "beerkegsontap", component: BeerKegsOnTapComponent },
            { path: "beerkegweights", component: BeerKegWeightsComponent },
            { path: "pub/:id", component: PubComponent },
            { path: "users", component: UsersComponent },
            { path: "pub-admins", component: PubAdminsComponent },
            { path: "brewery-admins", component: BreweryAdminsComponent },
            { path: "**", redirectTo: "home" }
        ])
    ],
    entryComponents: [
        SelectorComponent
    ],
    providers: [
        LocalizationConfig,
        {
            provide: APP_INITIALIZER, // APP_INITIALIZER will execute the function when the app is initialized and delay what it provides.
            useFactory: initLocalization,
            deps: [LocalizationConfig],
            multi: true
        }
    ],
})
export class AppModule {
}
