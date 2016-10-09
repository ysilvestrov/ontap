import { Component } from '@angular/core';
import { LoginService } from "./login.service";
import { UserService } from "../users/users.service";
import {IUser} from "../../models/ontap.models";
import { FormsModule }   from '@angular/forms';

@Component({
    selector: 'log-in',
    template: require('./login.component.html'),
    styles: [require('./login.component.css')],
    providers: [UserService],
})
export class LoginComponent {
    public isAuthorized: boolean;
    public errorMessage: any;
    public user: IUser;
    public userLogin: string;
    public userPassword: string; 

    constructor(private loginService: LoginService, private userService: UserService) {
        if (loginService.isAuthorised()) {
            this.isAuthorized = true;
            this.userLogin = this.parseJwt(loginService.accessToken.accessToken).sub;
            this.getUser(this.userLogin);            
        }
    }

    parseJwt(token) {
        var base64Url = token.split('.')[1];
        var base64 = base64Url.replace('-', '+').replace('_', '/');
        return JSON.parse(window.atob(base64));
    };

    login() {
        this.loginService.login(this.userLogin, this.userPassword)
            .subscribe(
            token => {
                this.isAuthorized = true;
                this.loginService.accessToken = token;
                if (sessionStorage) {
                    sessionStorage.setItem("token", JSON.stringify(token));
                }
                this.getUser(this.userLogin);
            },
            error => {
                this.errorMessage = <any>error;
                this.isAuthorized = false;
                this.user = null;
            });
    }

    getUser(userLogin: string) {
        this.userService.getOne(userLogin)
            .subscribe(
            user => {
                this.user = user;
                this.loginService.currentUser = user;
            },
            error => {
                this.errorMessage = <any>error;
            }
            );
    }
}
