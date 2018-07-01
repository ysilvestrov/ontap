import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { IPubServe, PubServe, City } from "../../models/ontap.models";
import { AppService } from "../../modules/appComponent";
import { Observable } from 'rxjs/Observable';
import Loginservice = require("../login/login.service");

@Injectable()
export class PubService extends AppService<IPubServe> {
    constructor(http: Http, loginService: Loginservice.LoginService) {
        super(http, loginService);
        this.serverUrl = "api/pubserves";
    }

    default(): IPubServe {
        return new PubServe({
            id: 'id',
            name: 'name',
            address: 'address',
            image: '',
            taplistHeaderImage: '',
            taplistFooterImage: '',
            city: new City({ id: 'kyiv', name: 'Kyiv' }),
            serves: [],
            lastUpdated: new Date(),
            facebookUrl: '',
            vkontakteUrl: '',
            websiteUrl: '',
            bookingUrl: '',
            parserOptions: '',
            tapNumber: 0,
        });
    }

    new(source: IPubServe): IPubServe {
        return new PubServe(source);
    }

    copy(source: IPubServe, dest: IPubServe) {
        dest.name = source.name;
        dest.city = source.city;
        dest.address = source.address;
        dest.serves = source.serves;
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