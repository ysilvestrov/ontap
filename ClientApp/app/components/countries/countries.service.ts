import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {ICountry} from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import Loginservice = require("../login/login.service");

@Injectable()
export class CountryService extends AppService<ICountry> {
    constructor(http: Http, loginService: Loginservice.LoginService) {
        super(http, loginService);
        this.serverUrl = "api/countries";
    }

    default(): ICountry {
        return {
            id: 'id',
            name: 'name'
        };
    }

    new(source: ICountry): ICountry {
        var country = this.default();
        this.copy(source, country);
        return country;
    }

    copy(source: ICountry, dest: ICountry) {
        dest.name = source.name;
    }
}