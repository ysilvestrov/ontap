import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IBrewery, Brewery , ICountry} from "../../models/ontap.models.ts";
import {BreweryService} from "./breweries.service.ts";
import {CountryService} from "../countries/countries.service.ts";
import {AppComponent, AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'breweries',
    providers: [BreweryService, CountryService],
    template: require('./breweries.component.html')
})
export class BreweriesComponent extends AppComponent<IBrewery, BreweryService> {

    public countries: ICountry[];

    constructor(elmService: BreweryService, private countryService: CountryService) {
        super(elmService);
        this.getCountries();
    }

    startAdd() {
        super.startAdd();
        this.adding.country = new List(this.countries).First();
    }

    getCountries() {
        this.countryService.get()
            .subscribe(
            countries => this.countries = countries,
            error => this.errorMessage = <any>error);
    }

    onEditChangeCountry(id) {
        this.onChangeCountry(this.editing, id);
    }

    onAddChangeCountry(id) {
        this.onChangeCountry(this.adding, id);
    }

    onChangeCountry(obj: IBrewery, id: string) {
        obj.country = new List(this.countries).Where(c => c.id === id).First();
    }

}