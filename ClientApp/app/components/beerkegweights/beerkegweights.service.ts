import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBeerKegWeight, BeerKegWeight } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BeerKegWeightsService extends AppService<IBeerKegWeight> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/beerkegweights";
    }

    default(): IBeerKegWeight {
        return new BeerKegWeight({
            date: new Date(),
            weight: 30
    });
    }

    new(source: BeerKegWeight): BeerKegWeight {
        return new BeerKegWeight(source);
    }

}