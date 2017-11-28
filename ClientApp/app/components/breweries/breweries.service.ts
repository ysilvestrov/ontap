import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {IBrewery, Brewery} from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BreweryService extends AppService<IBrewery> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/breweries";
    }

    default(): IBrewery {
        return new Brewery({
            id: 'id',
            name: 'name',
            address: 'address',
            image: '',
            country: {id: 'UA', name: 'Ukraine'},
        });
    }

    new(source: IBrewery): IBrewery {
        return new Brewery(source);
    }
}