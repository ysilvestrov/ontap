import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {ICity, City} from "../../models/ontap.models.ts";
import {AppService} from "../../modules/appComponent.ts";

@Injectable()
export class CityService extends AppService<ICity> {
    constructor(http: Http) {
        super(http);
        this.serverUrl = "api/cities";
    }

    default(): ICity {
        return new City({
            id: 'id',
            name: 'name'
        });;
    }

    new(source: ICity): ICity {
        return new City(source);
    }

    copy(source: ICity, dest: ICity) {
        dest.name = source.name;
    }
}