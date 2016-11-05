import { Component } from '@angular/core';
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@Component({
    selector: 'home',
    template: require('./home.component.html')
})
export class HomeComponent extends Locale {

    public isBrowser: boolean;
    public isLoaded: boolean;

    constructor(public locale: LocaleService, public localization: LocalizationService) {
        super(locale, localization);
        this.isBrowser = typeof (document) != "undefined";
        this.isLoaded = false;
        this.localization.translationChanged.subscribe(

            // Refreshes the variable 'title' with the new translation when the selected language changes.
            () => { this.isLoaded = true; }

        );
    }
}
