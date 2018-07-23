import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBeerKegOnTap, BeerKegOnTap, KegStatus } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class BeerKegsOnTapService extends AppService<IBeerKegOnTap> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/beerkegsontap";
    }

    default(): IBeerKegOnTap {
        return new BeerKegOnTap({
            installTime: new Date(),
            priority:0
    });
    }

    new(source: BeerKegOnTap): BeerKegOnTap {
        return new BeerKegOnTap(source);
    }

}