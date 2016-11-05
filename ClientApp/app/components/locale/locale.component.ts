import { Component } from "@angular/core";
// Services.
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@Component({
    selector: "locale",
    template: require("./locale.component.html"),
    styles: [require("./locale.component.css")],
})

export class LocaleComponent {

    currentLocale:string;

    constructor(public locale: LocaleService, public localization: LocalizationService) {
        this.currentLocale = this.locale.getCurrentLanguage();
    }

    // Sets a new locale & currency.
    selectLocale(language: string, country: string/*, currency: string*/): void {

        this.locale.setCurrentLocale(language, country);
        this.currentLocale = this.locale.getCurrentLanguage();
        //this.locale.setCurrentCurrency(currency);

    }

}