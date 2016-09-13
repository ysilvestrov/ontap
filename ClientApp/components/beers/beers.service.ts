import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {IBeer, Beer} from "../../models/ontap.models.ts";
import {AppService} from "../../modules/appComponent.ts";

@Injectable()
export class BeerService extends AppService<IBeer> {
    constructor(http: Http) {
        super(http);
        this.serverUrl = "api/beers";
    }

    default(): IBeer {
        return new Beer({
            id: 'id',
            name: 'name',
            description: 'description',
            brewery: 'brewery',
            type: 'Lager',
            alcohol: 2.5,
            ibu: 30,
            gravity: 15,
        });
    }

    new(source: IBeer): IBeer {
        return new Beer(source);
    }

    copy(source: IBeer, dest: IBeer) {
        dest.name = source.name;
        dest.description = source.description;
        dest.brewery = source.brewery;
    }
}