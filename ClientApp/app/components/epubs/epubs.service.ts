﻿import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { IPub, Pub, City } from "../../models/ontap.models";
import { AppService } from "../../modules/appComponent";
import { Observable } from 'rxjs/Observable';
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
            taplistHeaderImage: '',
            taplistFooterImage: '',
            city: new City({ id: 'kyiv', name: 'Kyiv' }),          
            facebookUrl: '',
            vkontakteUrl: '',
            websiteUrl: '',
            bookingUrl: '',
            parserOptions: '',
            tapNumber: 0,
        });
    }

    new(source: IPub): IPub {
        return new Pub(source);
    }

    copy(source:IPub, dest: IPub) {
        dest.name = source.name;
        dest.city = source.city;
        dest.address = source.address;
        dest.image = source.image;
        dest.taplistHeaderImage = source.taplistHeaderImage;
        dest.taplistFooterImage = source.taplistFooterImage;
        dest.facebookUrl = source.facebookUrl;
        dest.bookingUrl = source.facebookUrl;
        dest.vkontakteUrl = source.vkontakteUrl;
        dest.websiteUrl = source.websiteUrl;
        dest.parserOptions = source.parserOptions;
        dest.tapNumber = source.tapNumber;
    }

    private extractString(res: Response) {
        let body = res.text();
        return body || "";
    }

    import(id: string): Observable<string> {
        let body = "";
        return this.http.patch(this.serverUrl + "/" + id, body, this.options)
            .map(this.extractString, this)
            .catch(this.handleError);
    }

    importAll(): Observable<string> {
        let body = "";
        return this.http.patch(this.serverUrl, body, this.options)
            .map(this.extractString, this)
            .catch(this.handleError);
    }
}