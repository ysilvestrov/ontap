import { Component } from '@angular/core';
import { LoginService } from "../login/login.service";
import { NavigationEnd, Router } from '@angular/router';
import '../../modules/rxjs.operators';

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

    constructor(public router: Router) {
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