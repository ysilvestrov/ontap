import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IBeer, Beer} from "../../models/ontap.models.ts";
import {BeerService} from "./beers.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'beers',
    providers: [BeerService],
  template: require('./beers.html')
})
export class Beers extends AppComponent<IBeer, BeerService> {

    constructor(elmService: BeerService) {
        super(elmService);
    }
}