import { Injectable }     from '@angular/core';
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
            city: new City({ id: 'kyiv', name: 'Kyiv' }),
            serves: [],
            facebookUrl: '',
            vkontakteUrl: '',
            websiteUrl: '',
            bookingUrl: '',
            parserOptions: '',
        });
    }

    new(source: IPub): IPub {
        return new Pub(source);
    }

    copy(source:IPub, dest: IPub) {
        dest.name = source.name;
        dest.city = source.city;
        dest.address = source.address;
        dest.serves = source.serves;
        dest.image = source.image;
        dest.facebookUrl = source.facebookUrl;
        dest.bookingUrl = source.facebookUrl;
        dest.vkontakteUrl = source.vkontakteUrl;
        dest.websiteUrl = source.websiteUrl;
        dest.parserOptions = source.parserOptions;
    }

    private extractString(res: Response) {
        let body = res.text();
        return body || "";
    }

    import(id: string): Observable<string> {
        let body = "";
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this.serverUrl + "/" + id, body, options)
            .map(this.extractString, this)
            .catch(this.handleError);
    }
}