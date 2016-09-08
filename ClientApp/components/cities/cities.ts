import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {ICity, City} from "../../models/ontap.models.ts";
import {CityService} from "./cities.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'cities',
    providers: [CityService],
  template: require('./cities.html')
})
export class Cities extends AppComponent<ICity, CityService> {

    constructor(elmService: CityService) {
        super(elmService);
    }
}