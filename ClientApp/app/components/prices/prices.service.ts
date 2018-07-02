import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBeerPrice, BeerPrice } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BeerPriceService extends AppService<IBeerPrice> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/prices";
    }

    default(): IBeerPrice {
        return new BeerPrice({
            id: 1,
            price: 50.0,
            volume: 0.5,
            validFrom: new Date(),
        });
    }

    new(source: IBeerPrice): IBeerPrice {
        return new BeerPrice(source);
    }

}