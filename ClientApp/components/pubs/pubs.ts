import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, ICity} from "../../models/ontap.models.ts";
//var $ = require('jquery');

@ng.Component({
  selector: 'pubs',
  template: require('./pubs.html')
})
export class Pubs implements ng.OnInit {
    public pubs: IPub[];
    public allPubs: IPub[];
    public cities: ICity[];
    public city: ICity;

    constructor(http: Http) {
        http.get('/api/pubs').subscribe(result => {
            var pubs = result.json();
            this.allPubs = this.pubs = pubs;
            this.city = null;
            this.cities = new List(pubs)
                .Select((pub: IPub) => pub.city)
                .Distinct()
                .OrderBy((city: ICity) => city.name)
                .ToArray();
        });
    }

    ngOnInit() {
        //setTimeout(()=>($('[data-toggle="tooltip"]').tooltip()), 1000);        
    }

    public setCity(name) {
        this.city = new List(this.cities)
            .Where((city) => city.name === name)
            .First();
        this.pubs = (name === "") ? this.allPubs :
            new List(this.allPubs)
            .Where((pub) => pub.city.name === this.city.name)
            .ToArray();
    }
}
