import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBrewery, IBeer, IBreweryAdmin, BreweryAdmin, User, Pub, City, Brewery, ICountry } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BreweryAdminService extends AppService<IBreweryAdmin> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/breweryadmins";
    }

    default(): IBreweryAdmin {
        return new BreweryAdmin({
            id: 0,
            user: new User({
                id: 'id',
                name: 'name',
                password: '',
                isAdmin: false,
                canAdminBrewery: false,
                canAdminPub: false,
                breweries: [],
                pubs: []
            }),
            brewery: new Brewery({
                id: 'id',
                name: 'name',
                address: 'address',
                image: '',
                country: {id: 'UA', name: 'Ukraine'}
            })
        });
    }

    new(source: IBreweryAdmin): IBreweryAdmin {
        return new BreweryAdmin(source);
    }

    copy(source:IBreweryAdmin, dest: IBreweryAdmin) {
        dest.brewery = source.brewery;
        dest.user = source.user;
    }
}