import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IBeerServedInPub } from "../../models/ontap.models";
import {RoAppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class ServeService extends RoAppService<IBeerServedInPub> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/serves";
    }
}