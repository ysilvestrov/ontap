import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IKeg, Keg } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class KegService extends AppService<IKeg> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/kegs";
    }

    default(): IKeg {
        return new Keg({
            volume: 30,
            emptyWeight: 5,
            isReturnable: false      
        });
    }

    new(source: IKeg): IKeg {
        return new Keg(source);
    }

}