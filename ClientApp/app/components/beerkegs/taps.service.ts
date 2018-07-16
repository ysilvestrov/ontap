import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBeerKeg, BeerKeg, KegStatus } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class TapService extends AppService<IBeerKeg> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/taps";
    }

    default(): IBeerKeg {
        return new BeerKeg({
            arrivalDate: new Date(),
            status: KegStatus.Waiting
    });
    }

    new(source: IBeerKeg): IBeerKeg {
        return new BeerKeg(source);
    }

}