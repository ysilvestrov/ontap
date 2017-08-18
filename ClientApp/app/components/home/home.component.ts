import { Component } from '@angular/core';
import { Locale, LocaleService, LocalizationService, ServiceState } from "angular2localization";
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    selector: 'home',
    template: require('./home.component.html')
})
export class HomeComponent extends Locale {

    public isBrowser: boolean;
    public isLoaded: boolean;

    constructor(public locale: LocaleService, public localization: LocalizationService,
        private route: ActivatedRoute,
        private router: Router) {
        super(locale, localization);
        this.isBrowser = typeof (document) != "undefined";
        this.isLoaded = this.localization.serviceState === ServiceState.isReady;
        this.localization.translationChanged.subscribe(

            // Refreshes the variable 'title' with the new translation when the selected language changes.
            () => { this.isLoaded = true; }

        );
    }

    goToPubs() {
        var location = ['/pubs'];
        if (sessionStorage) {
            sessionStorage.setItem("is18", "true");
            var storedLocation = sessionStorage.getItem("goTo");
            if (storedLocation) {
                location = JSON.parse(storedLocation);
            }
        }
        this.router.navigate(location);
    }
}
