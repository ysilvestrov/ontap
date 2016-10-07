import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {IUser, User} from "../../models/ontap.models.ts";
import {AppService} from "../../modules/appComponent.ts";

@Injectable()
export class UserService extends AppService<IUser> {
    constructor(http: Http) {
        super(http);
        this.serverUrl = "api/users";
    }

    default(): IUser {
        return new User({
            id: 'id',
            name: 'name',
            password: '',
            isAdmin: false,
            canAdminBrewery: false,
            canAdminPub: false,
            pubs: [],
            breweries: []
        });
    }

    new(source: IUser): IUser {
        return new User(source);
    }

    copy(source: IUser, dest: IUser) {
        dest.name = source.name;
        dest.password = source.password;
        dest.isAdmin = source.isAdmin;
        dest.canAdminPub = source.canAdminPub;
        dest.canAdminBrewery = source.canAdminBrewery;
    }
}