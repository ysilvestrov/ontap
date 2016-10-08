import { Component } from '@angular/core';
import { LoginService } from "../login/login.service";

@Component({
    selector: 'app',
    template: require('./app.component.html'),
    styles: [require('./app.component.css')],
    providers: [LoginService],
})
export class AppComponent {
}
