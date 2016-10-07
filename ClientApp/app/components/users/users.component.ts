import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IUser, User} from "../../models/ontap.models.ts";
import {UserService} from "./users.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'users',
    providers: [UserService],
  template: require('./users.component.html')
})
export class UsersComponent extends AppComponent<IUser, UserService> {

    constructor(elmService: UserService) {
        super(elmService);
    }
}