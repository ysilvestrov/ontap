import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { FormsModule }   from '@angular/forms';
import { List } from "../../modules/linq.ts";
import {IBeer, Beer, IBrewery} from "../../models/ontap.models.ts";
import {BeerService} from "./beers.service.ts";
import {BreweryService} from "../breweries/breweries.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'beers',
    providers: [BeerService, BreweryService],
  template: require('./beers.component.html')
})
export class BeersComponent extends AppComponent<IBeer, BeerService> {

    public breweries: IBrewery[];

    constructor(elmService: BeerService, private breweryService: BreweryService) {
        super(elmService);
        this.getBreweries();
    }

    startAdd() {
        super.startAdd();
        this.adding.brewery = new List(this.breweries).First();
    }

    getBreweries() {
        this.breweryService.get()
            .subscribe(
            breweries => this.breweries = breweries,
            error => this.errorMessage = <any>error);
    }

    onEditChangeBrewery(id) {
        this.onChangeBrewery(this.editing, id);
    }

    onAddChangeBrewery(id) {
        this.onChangeBrewery(this.adding, id);
    }

    onChangeBrewery(obj: IBeer, id: string) {
        obj.brewery = new List(this.breweries).Where(c => c.id === id).First();
    }

}