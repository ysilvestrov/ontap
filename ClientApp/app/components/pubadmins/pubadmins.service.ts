import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IPub, IBeer, IPubAdmin, PubAdmin, User, Pub, City, Brewery } from "../../models/ontap.models";
import {AppService} from "../../modules/appComponent";
import { LoginService } from "../login/login.service";

@Injectable()
export class PubAdminService extends AppService<IPubAdmin> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/pubadmins";
    }

    default(): IPubAdmin {
        return new PubAdmin({
            id: 0,
            user: new User({
                id: 'id',
                name: 'name',
                password: '',
                isAdmin: false,
                canAdminBrewery: false,
                canAdminPub: false,
                breweries: [],
                pubs: []
            }),
            pub: new Pub({
                id: 'id',
                name: 'name',
                address: 'address',
                image: '',
                taplistHeaderImage: '',
                taplistFooterImage: '',
                city: new City({ id: 'kyiv', name: 'Kyiv' }),
                serves: [],
                facebookUrl: '',
                vkontakteUrl: '',
                websiteUrl: '',
                bookingUrl: '',
                parserOptions: '',
                tapNumber: 0,
            })
        });
    }

    new(source: IPubAdmin): IPubAdmin {
        return new PubAdmin(source);
    }

    copy(source:IPubAdmin, dest: IPubAdmin) {
        dest.pub = source.pub;
        dest.user = source.user;
    }
}