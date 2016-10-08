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

    constructor(private loginService:LoginService, private userService:UserService) {
    }

    login() {
        this.loginService.login(this.userLogin, this.userPassword)
            .subscribe(
            token => {
                this.isAuthorized = true;
                this.loginService.accessToken = token;
                this.userService.getOne(this.userLogin)
                    .subscribe(
                        user => {
                            this.user = user;
                            this.loginService.currentUser = user;
                        },
                        error => {
                            this.errorMessage = <any>error;
                        }
                    );
            },
            error => {
                this.errorMessage = <any>error;
                this.isAuthorized = false;
                this.user = null;
            });
    }
}
