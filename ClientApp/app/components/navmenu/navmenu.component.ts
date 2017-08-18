import { Component } from '@angular/core';
import { LoginService } from "../login/login.service";
import { UserService } from "../users/users.service";
import { Locale, LocaleService, LocalizationService } from 'angular2localization';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    selector: 'nav-menu',
    template: require('./navmenu.component.html'),
    styles: [require('./navmenu.component.css')],
    providers: [UserService],
})
export class NavMenuComponent extends Locale {

    public isBrowser: boolean;
    public isLoaded: boolean;

    constructor(private loginService: LoginService, private userService: UserService, public locale: LocaleService, public localization: LocalizationService) {
        super(locale, localization);

        this.isBrowser = typeof (document) != "undefined";
        this.isLoaded = false;
        this.localization.translationChanged.subscribe(

            // Refreshes the variable 'title' with the new translation when the selected language changes.
            () => { this.isLoaded = true; }

        );
    }
}
