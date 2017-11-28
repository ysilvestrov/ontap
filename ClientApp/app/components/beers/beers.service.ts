import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {IBeer, Beer, Brewery} from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BeerService extends AppService<IBeer> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/beers";
    }

    default(): IBeer {
        return new Beer({
            id: 'id',
            name: 'name',
            description: 'description',
            brewery: new Brewery({ id: '', name: '', address: '', image:'', country: { id: 'UA', name: 'Ukraine' } }),
            type: 'Lager',
            alcohol: 2.5,
            ibu: 30,
            gravity: 15,
            bjcpStyle: '',
            image: '',
        });
    }

    new(source: IBeer): IBeer {
        return new Beer(source);
    }

}