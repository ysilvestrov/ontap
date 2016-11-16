import { Injectable }     from '@angular/core';
import { Http } from '@angular/http';
import { IPub, IBeer, IServe, Serve, Beer, Pub, City, Brewery } from "../../models/ontap.models.ts";
import {AppService} from "../../modules/appComponent.ts";
import { LoginService } from "../login/login.service";

@Injectable()
export class ServeService extends AppService<IServe> {
    constructor(http: Http, loginService: LoginService) {
        super(http, loginService);
        this.serverUrl = "api/serves";
    }

    default(): IServe {
        return new Serve({
            id: 0,
            served: new Beer({
                id: 'id',
                name: 'name',
                description: 'description',
                brewery: new Brewery({id:'', name:'', address: '', image:'', country: {id: 'UA', name: 'Ukraine'}}),
                type: 'Lager',
                alcohol: 2.5,
                ibu: 30,
                gravity: 15,
            }),
            servedIn: new Pub({
                id: 'id',
                name: 'name',
                address: 'address',
                image: '',
                city: new City({ id: 'kyiv', name: 'Kyiv' }),
                serves: [],
            }),
            price: 0,
            tap: 1,
            volume: 0.5,
            updated: new Date(),
        });
    }

    new(source: IServe): IServe {
        return new Serve(source);
    }

    copy(source:IServe, dest: IServe) {
        dest.served = source.served;
        dest.servedIn = source.servedIn;
        dest.price = source.price;
        dest.tap = source.tap;
        dest.volume = source.volume;
        dest.updated = source.updated;
    }
}