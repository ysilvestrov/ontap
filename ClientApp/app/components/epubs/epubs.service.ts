import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IPub, Pub, City } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import Loginservice = require("../login/login.service");

@Injectable()
export class EPubService extends AppService<IPub> {
    constructor(http: Http, loginService: Loginservice.LoginService) {
        super(http, loginService);
        this.serverUrl = "api/pubs";
    }

    default(): IPub {
        return new Pub({
            id: 'id',
            name: 'name',
            address: 'address',
            image: '',
            city: new City({ id: 'kyiv', name: 'Kyiv' }),
            serves: [],
        });
    }

    new(source: IPub): IPub {
        return new Pub(source);
    }

    copy(source:IPub, dest: IPub) {
        dest.name = source.name;
        dest.city = source.city;
        dest.address = source.address;
        dest.serves = source.serves;
        dest.image = source.image;
    }
}