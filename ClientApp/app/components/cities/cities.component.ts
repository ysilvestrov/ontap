import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {ICity, City} from "../../models/ontap.models";
import {CityService} from "./cities.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'cities',
    providers: [CityService],
  template: require('./cities.component.html')
})
export class CitiesComponent extends AppComponent<ICity, CityService> {

    constructor(elmService: CityService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
    }
}