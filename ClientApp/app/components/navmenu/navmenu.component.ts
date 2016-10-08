import { Component } from '@angular/core';
import { LoginService } from "../login/login.service";
import { UserService } from "../users/users.service";

@Component({
    selector: 'nav-menu',
    template: require('./navmenu.component.html'),
    styles: [require('./navmenu.component.css')],
    providers: [UserService],
})
export class NavMenuComponent {
    constructor(private loginService: LoginService, private userService: UserService) {
    }
}
