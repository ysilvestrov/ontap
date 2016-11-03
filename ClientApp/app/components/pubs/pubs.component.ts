import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, ICity, Pub, IServe} from "../../models/ontap.models.ts";
import {EPubService} from "../epubs/epubs.service.ts";
import {CityService} from "../cities/cities.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";
import {SortByTap} from "../app/sortbytap.pipe";
import { TooltipContainerComponent, TooltipDirective, TooltipModule, Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';
import * as moment from 'moment';

@ng.Component({
    selector: 'pubs',
    providers: [EPubService],
    styles: [require('./pubs.component.css')],
  template: require('./pubs.component.html')
})
export class PubsComponent extends AppComponent<IPub, EPubService> implements ng.OnInit {
    public allPubs: IPub[];
    public cities: ICity[];
    public city: ICity;
    public pubUpdates: {};

    constructor(elmService: EPubService) {
        super(elmService);
        if (this.elements) {
            this.onElementsLoad(this.elements);
        }
        this.onLoad.subscribe((s: PubsComponent, elements:IPub[]) => {this.onElementsLoad(elements)});
    }

    ngOnInit() {
    }

    onElementsLoad(elements:IPub[]) {
        this.allPubs = elements;
        this.city = null;
        var pubCities = new List(this.allPubs)
            .Select((pub: IPub) => pub.city).ToArray();
        this.cities = new List(pubCities)
            .DistinctBy((city: ICity) => city.name)
            .OrderBy((city: ICity) => city.name)
            .ToArray();
        this.setCity("");
        var allDates = new List(this.allPubs).Select(p => [p.id, new List(p.serves).Select(s => s.updated).Max()]);
        var selectedDates = allDates.Where(t => typeof(t[1]) != "undefined" && t[1] && moment(t[1]).diff(moment().subtract(1, "year")) > 0);
        var d = allDates.ToArray();
        var d1 = selectedDates.ToArray();
        moment.locale("uk");
        this.pubUpdates = selectedDates.ToDictionary(t => t[0], t => moment(t[1]).calendar());
        
    }

    public setCity(name) {
        this.city = new List(this.cities)
            .Where((city) => city.name === name)
            .First();
        this.elements = (name === "") ? 
            new List(this.allPubs)
                .OrderByDescending((pub: IPub) => pub.serves.length)
                .ToArray() :
            new List(this.allPubs)
                .Where((pub) => pub.city.name === this.city.name)
                .OrderByDescending((pub:IPub) => pub.serves.length)
                .ToArray();
    }
}
