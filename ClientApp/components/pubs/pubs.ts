import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, ICity, Pub} from "../../models/ontap.models.ts";
import {EPubService} from "../epubs/epubs.service.ts";
import {CityService} from "../cities/cities.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'pubs',
    providers: [EPubService, CityService],
  template: require('./pubs.html')
})
export class Pubs extends AppComponent<IPub, EPubService> implements ng.OnInit {
    public allPubs: IPub[];
    public cities: ICity[];
    public city: ICity;

    constructor(elmService: EPubService, private cityService: CityService) {
        super(elmService);
        if (this.elements) {
            this.onLoad1(this.elements);
        }
        this.onLoad.subscribe((s:Pubs, elements:IPub[]) => {this.onLoad1(elements)});
    }

    ngOnInit() {
    }

    onLoad1(elements:IPub[]) {
            this.allPubs = elements;
            this.city = null;
            this.cities = new List(this.allPubs)
                .Select((pub: IPub) => pub.city)
                .Distinct()
                .OrderBy((city: ICity) => city.name)
                .ToArray();
    }

    public setCity(name) {
        this.city = new List(this.cities)
            .Where((city) => city.name === name)
            .First();
        this.elements = (name === "") ? this.allPubs :
            new List(this.allPubs)
                .Where((pub) => pub.city.name === this.city.name)
                .ToArray();
    }
}
