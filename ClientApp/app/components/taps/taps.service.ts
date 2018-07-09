import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { ITap, Tap } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class TapService extends AppService<ITap> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/taps";
    }

    default(): ITap {
        return new Tap({
            id: 1,
            number: "1",
            fitting: "A",
            hasHopinator: false,
            nitrogenPercentage: 0,
            status: 1         
        });
    }

    new(source: ITap): ITap {
        return new Tap(source);
    }

}