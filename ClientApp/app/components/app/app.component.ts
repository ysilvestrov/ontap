import { Component } from '@angular/core';
import { LoginService } from "../login/login.service";
import { NavigationEnd, Router } from '@angular/router';
import '../../modules/rxjs.operators';
import { Locale, LocaleService, LocalizationService } from "angular2localization";

// Declare ga function as ambient
declare var ga: Function;

@Component({
    selector: 'app',
    template: require('./app.component.html'),
    styles: [require('./app.component.css')],
    providers: [LoginService],
})
export class AppComponent {
    private currentRoute: string;

    constructor(public router: Router, public locale: LocaleService, public localization: LocalizationService) {

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
        this.localization.translationProvider("./resources/locale-"); // Required: initializes the translation provider with the given path prefix.
        this.localization.updateTranslation(); // Need to update the translation.

        router.events.distinctUntilChanged((previous: any, current: any) => {
            if (current instanceof NavigationEnd) {
                return previous.url === current.url;
            }
            return true;
        }).subscribe((x: any) => {
            console.log('router.change', x);
            if (typeof(ga) != "undefined") {
                ga || ga('send', 'pageview', x.url);
            }
        });
    }
}